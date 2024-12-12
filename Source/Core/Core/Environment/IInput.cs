namespace DotNetToolbox.Environment;

public interface IInput {
    Encoding Encoding { get; set; }
    TextReader Reader { get; }

    TextPromptBuilder BuildTextPrompt(string prompt);
    ValuePromptBuilder<TValue> BuildInlinePrompt<TValue>(string prompt);
    SelectionPromptBuilder BuildSelectionPrompt(string prompt);
    SelectionPromptBuilder<TValue> BuildSelectionPrompt<TValue>(string prompt, Func<TValue, object> selectKey);
    SelectionPromptBuilder<TValue, TKey> BuildSelectionPrompt<TValue, TKey>(string prompt, Func<TValue, TKey> selectKey)
        where TKey : notnull;

    int Read();
    ConsoleKeyInfo ReadKey(bool intercept = false);
    string ReadLine();
    Task<string> ReadLineAsync(CancellationToken ct = default);
    string ReadText();
    Task<string> ReadTextAsync(CancellationToken ct = default);

    bool Confirm(string prompt, bool defaultChoice = true);
    Task<bool> ConfirmAsync(string prompt, CancellationToken ct = default);
    Task<bool> ConfirmAsync(string prompt, bool defaultChoice, CancellationToken ct = default);

    string Ask(string prompt);
    string Ask(string prompt, string defaultValue);
    Task<string> AskAsync(string prompt, CancellationToken ct = default);
    Task<string> AskAsync(string prompt, string defaultValue, CancellationToken ct = default);

    TValue Prompt<TValue>(string prompt);
    TValue Prompt<TValue>(string prompt, TValue defaultValue);
    TValue Prompt<TValue>(string prompt, IEnumerable<TValue> choices);
    TValue Prompt<TValue>(string prompt, TValue defaultChoice, IEnumerable<TValue> otherChoices);
    Task<TValue> PromptAsync<TValue>(string prompt, CancellationToken ct = default);
    Task<TValue> PromptAsync<TValue>(string prompt, TValue defaultValue, CancellationToken ct = default);
    Task<TValue> PromptAsync<TValue>(string prompt, IEnumerable<TValue> choices, CancellationToken ct = default);
    Task<TValue> PromptAsync<TValue>(string prompt, TValue defaultChoice, IEnumerable<TValue> otherChoices, CancellationToken ct = default);

    string Prompt(string prompt);
    string Prompt(string prompt, string defaultValue);
    string Prompt(string prompt, IEnumerable<string> choices);
    string Prompt(string prompt, string defaultChoice, IEnumerable<string> otherChoices);
    Task<string> PromptAsync(string prompt, CancellationToken ct = default);
    Task<string> PromptAsync(string prompt, string defaultValue, CancellationToken ct = default);
    Task<string> PromptAsync(string prompt, IEnumerable<string> choices, CancellationToken ct = default);
    Task<string> PromptAsync(string prompt, string defaultChoice, IEnumerable<string> otherChoices, CancellationToken ct = default);

    TValue? SelectOne<TValue>(string prompt, Func<TValue, object> selectKey, params TValue[] choices);
    TValue? SelectOne<TValue>(string prompt, Func<TValue, object> selectKey, TValue defaultChoice, params TValue[] otherChoices);
    Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, CancellationToken ct = default);
    Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, TValue defaultChoice, CancellationToken ct = default);
    Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, TValue[] choices, CancellationToken ct = default);
    Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, TValue defaultChoice, TValue[] otherChoices, CancellationToken ct = default);
    string? SelectOne(string prompt, params string[] choices);
    string? SelectOne(string prompt, string defaultChoice, params string[] otherChoices);
    Task<string?> SelectOneAsync(string prompt, string defaultChoice, CancellationToken ct = default);
    Task<string?> SelectOneAsync(string prompt, CancellationToken ct = default);
    Task<string?> SelectOneAsync(string prompt, string defaultChoice, string[] otherChoices, CancellationToken ct = default);
    Task<string?> SelectOneAsync(string prompt, string[] choices, CancellationToken ct = default);
}
