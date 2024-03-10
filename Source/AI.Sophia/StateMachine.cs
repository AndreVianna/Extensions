﻿using DotNetToolbox.AI.Agents;

namespace DotNetToolbox.Sophia;

public class StateMachine {
    private readonly IOutput _out;
    private readonly IFileSystem _io;
    private readonly IPromptFactory _promptFactory;
    private readonly IApplication _app;
    private readonly OpenAIChatHandlerFactory _chatHandlerFactory;
    private readonly MultipleChoicePrompt _mainMenu;

    private static readonly JsonSerializerOptions _fileSerializationOptions = new() {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
        IgnoreReadOnlyProperties = true,
    };

    private const string _chatsFolder = "Chats";
    private const string _agentsFolder = "Agents";
    private const string _skillsFolder = "Skills";

    public StateMachine(IApplication app, IChatHandlerFactory chatHandlerFactory) {
        _app = app;
        _chatHandlerFactory = (OpenAIChatHandlerFactory)chatHandlerFactory;
        _io = app.Environment.FileSystem;
        _out = app.Environment.Output;
        _promptFactory = app.PromptFactory;
        _io.CreateFolder(_chatsFolder);
        _mainMenu = _promptFactory.CreateMultipleChoiceQuestion("What do you want to do?", opt => {
            opt.AddChoice(2, "Create a new chat");
            opt.AddChoice(3, "Continue a existing chat");
            opt.AddChoice(4, "Delete an existing chat");
            opt.AddChoice(5, "Exit");
        });
    }

    public const uint Idle = 0;

    public Agent<OpenAIChatOptions>? Agent { get; set; }
    public OpenAIChatHandler? Handler { get; set; }
    public uint CurrentState { get; set; }

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
            case 6: return SendMessage(input, ct);
        }

        return Task.CompletedTask;
    }

    private Task ShowMainMenu(CancellationToken ct) {
        CurrentState = _mainMenu.Ask();
        return Process(string.Empty, ct);
    }

    private async Task Start(CancellationToken ct) {
        try {
            Agent = await LoadAgentProfile("TimeKeeper");
            Agent.Skills.AddRange(LoadSkills());
            Handler = _chatHandlerFactory.Create(new() {
                Model = "gpt-3.5-turbo-0125",
                AgentName = Agent.Name,
            });
            _io.CreateFolder($"{_chatsFolder}/{Handler.Chat.Id}");
            await SaveAgentData(Handler.Chat.Id, ct);
            _out.WriteLine($"Handler for chat  '{Handler.Chat.Id}' started.");
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

        Handler = await LoadAgentData(chatId, ct);
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

        var chatId = SelectChat("Select a chat to cancel:", folders);
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

    private async Task SendMessage(string input, CancellationToken ct) {
        _out.Write("- ");
        Handler!.Chat.Messages.Add(new("user", [new("text", input)]));
        if (!await Submit(ct)) _out.WriteLine("Error!");
        else foreach (var part in Handler.Chat.Messages[^1].Parts) _out.WriteLine(part.Value);
        CurrentState = Idle;
    }

    private async Task<bool> Submit(CancellationToken ct) {
        var isFinished = false;
        while (!isFinished) {
            var result = await Handler!.Submit(Agent!, ct);
            if (!result.IsOk) return false;
            isFinished = await CheckIfIsFinished(ct);
        }
        return true;
    }

    private Task<bool> CheckIfIsFinished(CancellationToken ct)
        => Task.FromResult(ct.IsCancellationRequested);

    private async Task SaveAgentData(string chatId, CancellationToken ct) {
        await using var chatFile = _io.OpenOrCreateFile($"{_chatsFolder}/{chatId}/agent.json");
        await JsonSerializer.SerializeAsync(chatFile, Handler, _fileSerializationOptions, ct);
    }

    private async Task<OpenAIChatHandler?> LoadAgentData(string chatId, CancellationToken ct) {
        await using var agentFile = _io.OpenFileAsReadOnly($"{_chatsFolder}/{chatId}/agent.json");
        return await JsonSerializer.DeserializeAsync<OpenAIChatHandler>(agentFile, _fileSerializationOptions, cancellationToken: ct);
    }

    private IEnumerable<Skill> LoadSkills() {
        var skills = _io.GetFiles(_skillsFolder);
        foreach (var skill in skills) yield return LoadSkill(skill);
    }

    private Skill LoadSkill(string name) {
        using var skillFile = _io.OpenOrCreateFile($"{_skillsFolder}/{name}.json");
        return JsonSerializer.Deserialize<Skill>(skillFile, _fileSerializationOptions)!;
    }

    private async Task<Agent<OpenAIChatOptions>> LoadAgentProfile(string profileName) {
        await using var agentFile = _io.OpenOrCreateFile($"{_agentsFolder}/{profileName}.json");
        return JsonSerializer.Deserialize<Agent<OpenAIChatOptions>>(agentFile, _fileSerializationOptions)!;
    }
}
