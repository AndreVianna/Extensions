﻿using Microsoft.Extensions.Logging;

namespace DotNetToolbox.Sophia;

public class StateMachine : IRequestSource {
    private const string _chatsFolder = "Chats";
    private const string _agentsFolder = "Agents";
    //private const string _skillsFolder = "Skills";
    public const uint Idle = 0;
    private bool _waitingResponse;

    private static readonly JsonSerializerOptions _fileSerializationOptions = new() {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        IgnoreReadOnlyProperties = true,
    };

    private readonly IOutput _out;
    private readonly IFileSystem _io;
    private readonly IPromptFactory _promptFactory;
    private readonly IApplication _app;
    private readonly IHttpClientProvider _httpClientProvider;
    private readonly MultipleChoicePrompt _mainMenu;
    private readonly World _world;
    private readonly ILogger<OpenAIRunner> _runnerLogger;

    private OpenAIAgent? _agent;
    private OpenAIRunner? _runner;
    private IChat? _chat;

    public StateMachine(IApplication app, IHttpClientProvider httpClientProvider, ILoggerFactory loggerFactory) {
        _app = app;
        _httpClientProvider = httpClientProvider;
        _runnerLogger = loggerFactory.CreateLogger<OpenAIRunner>();
        _io = app.Environment.FileSystem;
        _out = app.Environment.Output;
        _promptFactory = app.PromptFactory;
        _io.CreateFolder(_chatsFolder);
        _world = new(app.Environment);
        _mainMenu = _promptFactory.CreateMultipleChoiceQuestion("What do you want to do?", opt => {
            opt.AddChoice(2, "Create a new chat");
            opt.AddChoice(3, "Continue a existing chat");
            opt.AddChoice(4, "Delete an existing chat");
            opt.AddChoice(5, "Exit");
        });
    }

    public uint CurrentState { get; set; }
    public uint TotalTokens => _chat?.TotalTokens ?? 0;

    internal Task Start(uint initialState, CancellationToken ct) {
        CurrentState = initialState;
        return Process(string.Empty, ct);
    }

    internal Task Process(string input, CancellationToken ct) {
        switch (CurrentState) {
            case 1: return ShowMainMenu(ct);
            case 2: return Start(ct);
            case 3: return Resume(ct);
            case 4: Terminate(); break;
            case 5: _app.Exit(); break;
            case 6: SendMessage(input); break;
        }

        return Task.CompletedTask;
    }

    private Task ShowMainMenu(CancellationToken ct) {
        CurrentState = _mainMenu.Ask();
        return Process(string.Empty, ct);
    }

    private async Task Start(CancellationToken ct) {
        try {
            _agent = await LoadAgentProfile("TimeKeeper");
            _runner = new(_agent, _world, _httpClientProvider, _runnerLogger);
            _runner.Run(ct);
            _chat = new Chat(_app.Environment);
            await SaveChat(_chat, ct);
            _out.WriteLine("Runner started.");
            CurrentState = Idle;
        }
        catch (Exception ex) {
            Console.WriteLine(ex);
            throw;
        }
    }

    private async Task Resume(CancellationToken ct) {
        var folders = _io.GetFolders(_chatsFolder).ToArray();
        if (folders.Length == 0) {
            _out.WriteLine("No chat found.");
            CurrentState = 1;
            return;
        }

        var chatId = SelectChat("Select a chat to resume:", folders);
        if (chatId == string.Empty) {
            CurrentState = 1;
            return;
        }

        _chat = await LoadChat(chatId, ct);
        _out.WriteLine($"Resuming chat '{chatId}'.");
        CurrentState = Idle;
    }

    private void Terminate() {
        var folders = _io.GetFolders(_chatsFolder).ToArray();
        if (folders.Length == 0) {
            _out.WriteLine("No chat found.");
            CurrentState = 1;
            return;
        }

        var chatId = SelectChat("Select a chat to delete permanently:", folders);
        if (chatId == string.Empty) {
            CurrentState = 1;
            return;
        }

        _io.DeleteFolder($"{_chatsFolder}/{chatId}", true);
        _out.WriteLine($"chat '{chatId}' cancelled.");
        CurrentState = 1;
    }

    private string SelectChat(string question, string[] folders) {
        var chats = _promptFactory.CreateMultipleChoiceQuestion<string>(question, opt => {
            foreach (var folder in folders) opt.AddChoice(folder, folder);
            opt.AddChoice(string.Empty, "Back to the main menu.");
        });
        var chatId = chats.Ask();
        return chatId;
    }

    private void SendMessage(string input) {
        if (_runner is null) {
            CurrentState = 1;
            return;
        }
        _out.Write("- ");
        _chat!.Messages.Add(new("user", [new("text", input)]));
        _waitingResponse = true;
        _runner.ReceiveRequest(this, _chat);
        while (_waitingResponse) Task.Delay(100);
    }

    public void ProcessResponse(ResponsePackage response) {
        foreach (var part in response.Message.Parts) _out.WriteLine(part.Value);
        _waitingResponse = false;
    }

    private async Task SaveChat(IChat chat, CancellationToken ct) {
        _io.CreateFolder($"{_chatsFolder}/{chat.Id}");
        await using var chatFile = _io.OpenOrCreateFile($"{_chatsFolder}/{chat.Id}/chat.json");
        await JsonSerializer.SerializeAsync(chatFile, _chat, _fileSerializationOptions, ct);
    }

    private async Task<IChat> LoadChat(string chatId, CancellationToken ct) {
        await using var agentFile = _io.OpenFileAsReadOnly($"{_chatsFolder}/{chatId}/chat.json");
        var chat = await JsonSerializer.DeserializeAsync<Chat>(agentFile, _fileSerializationOptions, cancellationToken: ct);
        return chat!;
    }

    //private IEnumerable<Skill> LoadSkills() {
    //    var skills = _io.GetFiles(_skillsFolder);
    //    foreach (var skill in skills) yield return LoadSkill(skill);
    //}

    //private Skill LoadSkill(string name) {
    //    using var skillFile = _io.OpenOrCreateFile($"{_skillsFolder}/{name}.json");
    //    return JsonSerializer.Deserialize<Skill>(skillFile, _fileSerializationOptions)!;
    //}

    private async Task<OpenAIAgent> LoadAgentProfile(string profileName) {
        await using var agentFile = _io.OpenOrCreateFile($"{_agentsFolder}/{profileName}.json");
        return JsonSerializer.Deserialize<OpenAIAgent>(agentFile, _fileSerializationOptions)!;
    }
}
