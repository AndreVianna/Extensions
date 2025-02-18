
namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for Console functionality.")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global - Used for externally.
public class ConsoleInput
    : HasDefault<ConsoleInput>,
      IInput {
    public virtual Encoding Encoding {
        get => Console.InputEncoding;
        set => Console.InputEncoding = value;
    }

    public virtual TextReader Reader => Console.In;

    public virtual int Read() => Reader.Read();
    public virtual ConsoleKeyInfo ReadKey(bool intercept = false) => Console.ReadKey(intercept);
    public virtual string ReadLine()
        => ReadLineAsync().GetAwaiter().GetResult();
    public virtual Task<string> ReadLineAsync(CancellationToken ct = default) {
        var builder = new ValuePromptBuilder<string>();
        return builder.ShowAsync(ct);
    }
    public virtual string ReadText()
        => ReadTextAsync().GetAwaiter().GetResult();
    public virtual Task<string> ReadTextAsync(CancellationToken ct = default) {
        var builder = new TextPromptBuilder();
        return builder.ShowAsync(ct);
    }

    public virtual bool Confirm(string prompt, bool defaultChoice = true)
        => ConfirmAsync(prompt, defaultChoice).GetAwaiter().GetResult();

    public virtual Task<bool> ConfirmAsync(string prompt, CancellationToken ct = default) {
        var builder = new ValuePromptBuilder<bool>(prompt);
        builder.AddChoices(true, false);
        builder.ConvertWith(value => value ? "y" : "n");
        return builder.ShowAsync(ct);
    }
    public virtual Task<bool> ConfirmAsync(string prompt, bool defaultChoice, CancellationToken ct = default) {
        var builder = new ValuePromptBuilder<bool>(prompt, defaultChoice);
        builder.AddChoices(true, false);
        builder.ConvertWith(value => value ? "y" : "n");
        return builder.ShowAsync(ct);
    }

    public virtual TValue Prompt<TValue>(string prompt)
        => PromptAsync<TValue>(prompt).GetAwaiter().GetResult();
    public virtual TValue Prompt<TValue>(string prompt, TValue defaultValue)
        => PromptAsync(prompt, defaultValue).GetAwaiter().GetResult();
    public virtual TValue Prompt<TValue>(string prompt, IEnumerable<TValue> choices)
        => PromptAsync(prompt, choices).GetAwaiter().GetResult();
    public virtual TValue Prompt<TValue>(string prompt, TValue defaultChoice, IEnumerable<TValue> otherChoices)
        => PromptAsync(prompt, defaultChoice, otherChoices).GetAwaiter().GetResult();

    public virtual Task<TValue> PromptAsync<TValue>(string prompt, CancellationToken ct = default) {
        var builder = new ValuePromptBuilder<TValue>(prompt);
        return builder.ShowAsync(ct);
    }
    public virtual Task<TValue> PromptAsync<TValue>(string prompt, TValue defaultValue, CancellationToken ct = default) {
        var builder = new ValuePromptBuilder<TValue>(prompt, defaultValue);
        builder.ShowOptionalFlag();
        return builder.ShowAsync(ct);
    }
    public virtual Task<TValue> PromptAsync<TValue>(string prompt, IEnumerable<TValue> choices, CancellationToken ct = default) {
        var builder = new ValuePromptBuilder<TValue>(prompt);
        builder.AddChoices(choices);
        return builder.ShowAsync(ct);
    }
    public virtual Task<TValue> PromptAsync<TValue>(string prompt, TValue defaultChoice, IEnumerable<TValue> otherChoices, CancellationToken ct = default) {
        var builder = new ValuePromptBuilder<TValue>(prompt, defaultChoice);
        builder.ShowOptionalFlag();
        builder.AddChoices([defaultChoice, .. otherChoices]);
        return builder.ShowAsync(ct);
    }

    public virtual TValue? SelectOne<TValue>(string prompt, Func<TValue, object> selectKey, params TValue[] choices)
        => SelectOneAsync(prompt, selectKey, choices).GetAwaiter().GetResult();
    public virtual TValue? SelectOne<TValue>(string prompt, Func<TValue, object> selectKey, TValue defaultChoice, params TValue[] otherChoices)
        => SelectOneAsync(prompt, selectKey, defaultChoice, otherChoices).GetAwaiter().GetResult();
    public virtual Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, CancellationToken ct = default)
        => SelectOneAsync(prompt, selectKey, [], ct);
    public virtual Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, TValue defaultChoice, CancellationToken ct = default)
        => SelectOneAsync(prompt, selectKey, defaultChoice, [], ct);
    public virtual Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, TValue[] choices, CancellationToken ct = default) {
        var builder = new SelectionPromptBuilder<TValue>(prompt, selectKey);
        if (choices.Length < 2) throw new ArgumentException("At least two choices must be provided.", nameof(choices));
        builder.AddChoices(choices);
        return builder.ShowAsync(ct);
    }
    public virtual Task<TValue?> SelectOneAsync<TValue>(string prompt, Func<TValue, object> selectKey, TValue defaultChoice, TValue[] otherChoices, CancellationToken ct = default) {
        var builder = new SelectionPromptBuilder<TValue>(prompt, selectKey);
        builder.AddDefaultChoice(defaultChoice);
        if (otherChoices.Length < 1) throw new ArgumentException("At least two choices must be provided.", nameof(otherChoices));
        builder.AddChoices(otherChoices);
        return builder.ShowAsync(ct);
    }

    public virtual string Ask(string prompt)
        => AskAsync(prompt).GetAwaiter().GetResult();
    public virtual string Ask(string prompt, string defaultValue)
        => AskAsync(prompt, defaultValue).GetAwaiter().GetResult();
    public virtual Task<string> AskAsync(string prompt, CancellationToken ct = default)
        => AskAsync(prompt, string.Empty, ct);
    public virtual Task<string> AskAsync(string prompt, string defaultValue, CancellationToken ct = default) {
        var builder = new TextPromptBuilder(prompt, defaultValue);
        return builder.ShowAsync(ct);
    }

    public virtual string Prompt(string prompt)
        => PromptAsync<string>(prompt).GetAwaiter().GetResult();
    public virtual string Prompt(string prompt, string defaultValue)
        => PromptAsync(prompt, defaultValue).GetAwaiter().GetResult();
    public virtual string Prompt(string prompt, IEnumerable<string> choices)
        => PromptAsync(prompt, choices).GetAwaiter().GetResult();
    public virtual string Prompt(string prompt, string defaultChoice, IEnumerable<string> otherChoices)
        => PromptAsync(prompt, defaultChoice, otherChoices).GetAwaiter().GetResult();
    public virtual Task<string> PromptAsync(string prompt, CancellationToken ct = default)
        => PromptAsync<string>(prompt, ct);
    public virtual Task<string> PromptAsync(string prompt, string defaultValue, CancellationToken ct = default)
        => PromptAsync<string>(prompt, defaultValue, ct);
    public virtual Task<string> PromptAsync(string prompt, IEnumerable<string> choices, CancellationToken ct = default)
        => PromptAsync<string>(prompt, choices, ct);
    public virtual Task<string> PromptAsync(string prompt, string defaultChoice, IEnumerable<string> otherChoices, CancellationToken ct = default)
        => PromptAsync<string>(prompt, defaultChoice, otherChoices, ct);

    public virtual string? SelectOne(string prompt, params string[] choices)
        => SelectOneAsync(prompt, choices).GetAwaiter().GetResult();
    public virtual string? SelectOne(string prompt, string defaultChoice, params string[] otherChoices)
        => SelectOneAsync(prompt, defaultChoice, otherChoices).GetAwaiter().GetResult();
    public virtual Task<string?> SelectOneAsync(string prompt, CancellationToken ct = default)
        => SelectOneAsync(prompt, [], ct);
    public virtual Task<string?> SelectOneAsync(string prompt, string defaultChoice, CancellationToken ct = default)
        => SelectOneAsync(prompt, defaultChoice, [], ct);
    public virtual Task<string?> SelectOneAsync(string prompt, string[] choices, CancellationToken ct = default) {
        var builder = new SelectionPromptBuilder(prompt);
        if (choices.Length < 2) throw new ArgumentException("At least two choices must be provided.", nameof(choices));
        builder.AddChoices(choices);
        return builder.ShowAsync(ct);
    }
    public virtual Task<string?> SelectOneAsync(string prompt, string defaultChoice, string[] otherChoices, CancellationToken ct = default) {
        var builder = new SelectionPromptBuilder(prompt);
        builder.AddDefaultChoice(defaultChoice);
        if (otherChoices.Length < 1) throw new ArgumentException("At least two choices must be provided.", nameof(otherChoices));
        builder.AddChoices(otherChoices);
        return builder.ShowAsync(ct);
    }

    public virtual TextPromptBuilder BuildTextPrompt(string prompt)
        => new(prompt);
    public virtual ValuePromptBuilder<TValue> BuildInlinePrompt<TValue>(string prompt)
        => new(prompt);

    public virtual SelectionPromptBuilder BuildSelectionPrompt(string prompt)
        => new(prompt);
    public virtual SelectionPromptBuilder<TValue> BuildSelectionPrompt<TValue>(string prompt, Func<TValue, object> selectKey)
        => new(prompt, selectKey);
    public virtual SelectionPromptBuilder<TValue, TKey> BuildSelectionPrompt<TValue, TKey>(string prompt, Func<TValue, TKey> selectKey)
        where TKey : notnull
        => new(prompt, selectKey);
}
