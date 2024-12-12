namespace DotNetToolbox.ConsoleApplication.TestDoubles;

internal sealed class TestInput(IOutput output, params string[] inputs)
    : IInput {
    private readonly Queue<string> _inputQueue = new(inputs);

    public int Read() => throw new NotImplementedException();

    public ConsoleKeyInfo ReadKey(bool intercept = false) => throw new NotImplementedException();

    public string ReadLine() {
        if (!_inputQueue.TryDequeue(out var input)) return string.Empty;
        output.WriteLine(input);
        return input;
    }
    public Task<string> ReadLineAsync(CancellationToken ct = default) => throw new NotImplementedException();

    public string ReadText() => string.Empty;
    public Task<string> ReadTextAsync(CancellationToken ct = default) => Task.FromResult(string.Empty);
    public string Prompt(string prompt) => string.Empty;
    public string Prompt(string prompt, string defaultValue) => throw new NotImplementedException();

    public string Prompt(string prompt, IEnumerable<string> choices) => throw new NotImplementedException();

    public string Prompt(string prompt, string defaultChoice, IEnumerable<string> otherChoices) => throw new NotImplementedException();

    public Task<string> PromptAsync(string prompt, CancellationToken ct = default) => Task.FromResult(string.Empty);
    public Task<string> PromptAsync(string prompt, string defaultChoice, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<string> PromptAsync(string prompt, IEnumerable<string> choices, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<string> PromptAsync(string prompt, string defaultChoice, IEnumerable<string> otherChoices, CancellationToken ct = default) => throw new NotImplementedException();

    public TextPromptBuilder BuildTextPrompt(string prompt) => throw new NotImplementedException();

    public ValuePromptBuilder<TValue> BuildInlinePrompt<TValue>(string prompt) => throw new NotImplementedException();
    public Task<bool> ConfirmAsync(string prompt, bool defaultChoice, CancellationToken ct = default) => throw new NotImplementedException();
    public TValue Ask<TValue>(string prompt) => throw new NotImplementedException();

    public TValue Ask<TValue>(string prompt, TValue defaultValue) => throw new NotImplementedException();

    public Task<TValue> AskAsync<TValue>(string prompt, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<TValue> AskAsync<TValue>(string prompt, TValue defaultChoice, CancellationToken ct = default) => throw new NotImplementedException();
    public TValue Prompt<TValue>(string prompt) => throw new NotImplementedException();

    public TValue Prompt<TValue>(string prompt, TValue defaultValue) => throw new NotImplementedException();

    public TValue Prompt<TValue>(string prompt, IEnumerable<TValue> choices) => throw new NotImplementedException();

    public TValue Prompt<TValue>(string prompt, TValue defaultChoice, IEnumerable<TValue> otherChoices) => throw new NotImplementedException();

    public Task<TValue> PromptAsync<TValue>(string prompt, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<TValue> PromptAsync<TValue>(string prompt, TValue defaultValue, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<TValue> PromptAsync<TValue>(string prompt, IEnumerable<TValue> choices, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<TValue> PromptAsync<TValue>(string prompt, TValue defaultChoice, IEnumerable<TValue> otherChoices, CancellationToken ct = default) => throw new NotImplementedException();

    public string Ask(string prompt, string defaultValue) => throw new NotImplementedException();

    public Task<string> AskAsync(string prompt, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<string> AskAsync(string prompt, string defaultChoice, CancellationToken ct = default) => throw new NotImplementedException();

    public bool Confirm(string prompt, bool defaultChoice = true) => throw new NotImplementedException();
    public Task<bool> ConfirmAsync(string prompt, CancellationToken ct = default) => throw new NotImplementedException();

    public SelectionPromptBuilder BuildSelectionPrompt(string prompt)
        => throw new NotImplementedException();
    public SelectionPromptBuilder<TValue> BuildSelectionPrompt<TValue>(string prompt, Func<TValue, object> selectKey)
        => throw new NotImplementedException();
    public SelectionPromptBuilder<TValue, TKey> BuildSelectionPrompt<TValue, TKey>(string prompt, Func<TValue, TKey> selectKey)
        where TKey : notnull
        => throw new NotImplementedException();
    public TValue SelectOne<TValue>(string prompt, Func<TValue, object> selectKey, TValue defaultChoice, params TValue[] otherChoices)
        => throw new NotImplementedException();
    public TValue SelectOne<TValue>(string prompt, Func<TValue, object> selectKey, params TValue[] choices)
        => throw new NotImplementedException();
    public string SelectOne(string prompt, string defaultChoice, params string[] otherChoices)
        => throw new NotImplementedException();
    public string SelectOne(string prompt, params string[] choices)
        => throw new NotImplementedException();
    public Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, TValue defaultChoice, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, TValue defaultChoice, TValue[] otherChoices, CancellationToken ct = default)
        => throw new NotImplementedException();

    public string Ask(string prompt) => throw new NotImplementedException();

    public Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, TValue[] choices, CancellationToken ct = default)
        => throw new NotImplementedException();
    public Task<string?> SelectOneAsync(string prompt, string defaultChoice, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<string?> SelectOneAsync(string prompt, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<string?> SelectOneAsync(string prompt, string defaultChoice, string[] otherChoices, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<string?> SelectOneAsync(string prompt, string[] choices, CancellationToken ct = default) => throw new NotImplementedException();
    public Encoding Encoding {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public TextReader Reader => throw new NotImplementedException();
}
