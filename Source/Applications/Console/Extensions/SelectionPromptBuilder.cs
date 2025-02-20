namespace DotNetToolbox.ConsoleApplication.Extensions;

public class SelectionPromptBuilder(string prompt)
    : SelectionPromptBuilder<string, string>(prompt, value => value);

public class SelectionPromptBuilder<TValue>(string prompt, Func<TValue, object> selectKey)
    : SelectionPromptBuilder<TValue, object>(prompt, selectKey);

public class SelectionPromptBuilder<TValue, TKey>(string prompt, Func<TValue, TKey> selectKey)
    : ISelectionPromptBuilder<TValue, TKey>
    where TKey : notnull {
    private record Choice(TValue? Value, TKey Key, string Text, ChoicePosition Position);

    private string _prompt = prompt;
    private readonly Func<TValue, TKey> _selectKey = IsNotNull(selectKey);
    private readonly List<Choice> _choices = [];

    private Func<TValue, string> _displayAs = v => v?.ToString() ?? string.Empty;
    private TKey _defaultKey = default!;
    private bool _showResult;

    [MemberNotNullWhen(true, nameof(_defaultKey))]
    private bool HasDefault { get; set; }

    public ISelectionPromptBuilder<TValue, TKey> DisplayAs(Func<TValue, string>? displayAs = null) {
        _displayAs = displayAs ?? _displayAs;
        return this;
    }

    public ISelectionPromptBuilder<TValue, TKey> ShowResult() {
        _showResult = true;
        return this;
    }

    public ISelectionPromptBuilder<TValue, TKey> SetAsDefault(TKey defaultKey) {
        if (!_choices.Any(c => c.Key.Equals(defaultKey))) throw new InvalidOperationException($"Choice with key '{defaultKey}' not found.");
        _defaultKey = IsNotNull(defaultKey);
        HasDefault = true;
        return this;
    }

    private ISelectionPromptBuilder<TValue, TKey> AddChoice(TValue? choice, TKey key, string? text, bool isDefault, ChoicePosition position = default) {
        if (_choices.Any(c => c.Key.Equals(IsNotNull(key)))) throw new InvalidOperationException("Choice key must be unique.");
        text ??= string.Empty;
        if (_choices.Any(c => c.Text.Equals(text, StringComparison.Ordinal))) throw new InvalidOperationException("Choice text must be unique.");
        var choiceIndex = position switch {
            ChoicePosition.AtEnd => _choices.Count,
            ChoicePosition.AtStart => 0,
            _ => GetInsertIndex(),
        };
        _choices.Insert(choiceIndex, new(choice, key, text, position));
        if (!isDefault) return this;
        if (HasDefault) throw new InvalidOperationException("Default choice is already set.");
        _defaultKey = key;
        HasDefault = true;
        return this;

        int GetInsertIndex() {
            var start = _choices.FindIndex(c => c.Position == ChoicePosition.Sorted);
            if (start == -1) start = _choices.Count;
            var sortedChoices = _choices.Where(c => c.Position == ChoicePosition.Sorted).ToList();
            var insertAt = sortedChoices.FindIndex(c => string.CompareOrdinal(c.Text, text) > 0);
            return insertAt == -1 ? start + sortedChoices.Count : start + insertAt;
        }
    }

    public ISelectionPromptBuilder<TValue, TKey> AddDefaultChoice(TKey key, string text, ChoicePosition position = default)
        => AddChoice(default, key, text, isDefault: true, position);

    public ISelectionPromptBuilder<TValue, TKey> AddDefaultChoice(TKey key, TValue choice, ChoicePosition position = default)
        => AddChoice(choice, key, _displayAs(choice), isDefault: true, position);

    public ISelectionPromptBuilder<TValue, TKey> AddDefaultChoice(TValue choice, ChoicePosition position = default)
        => AddChoice(choice, _selectKey(choice), _displayAs(choice), isDefault: true, position);

    public ISelectionPromptBuilder<TValue, TKey> AddChoice(TKey key, string text, ChoicePosition position = default)
        => AddChoice(default, key, text, isDefault: false, position);

    public ISelectionPromptBuilder<TValue, TKey> AddChoice(TKey key, TValue choice, ChoicePosition position = default)
        => AddChoice(choice, key, _displayAs(choice), isDefault: false, position);

    public ISelectionPromptBuilder<TValue, TKey> AddChoice(TValue choice, ChoicePosition position = default)
        => AddChoice(choice, _selectKey(choice), _displayAs(choice), isDefault: false, position);

    public ISelectionPromptBuilder<TValue, TKey> AddChoices(IEnumerable<TValue> choices, ChoicePosition position = default) {
        choices = position == ChoicePosition.AtStart ? choices.Reverse() : choices;
        foreach (var choice in choices) AddChoice(choice, position);
        return this;
    }

    public TValue? Show() => ShowAsync().GetAwaiter().GetResult();

    public async Task<TValue?> ShowAsync(CancellationToken ct = default) {
        var prompt = new SelectionPrompt<int>();
        prompt.UseConverter(i => _choices[i].Text);
        prompt.AddChoices(_choices.Select((_, i) => i));
        var defaultText = _choices.Find(i => i.Key.Equals(_defaultKey));
        var isQuestion = _prompt.EndsWith('?');
        _prompt = HasDefault ? $"{_prompt} [blue]({defaultText})[/]" : _prompt;
        prompt.Title(_prompt);
        var result = await prompt.ShowAsync(AnsiConsole.Console, ct);
        var resultText = prompt.Converter?.Invoke(result) ?? string.Empty;
        var separator = isQuestion ? string.Empty : ":";
        if (_showResult) AnsiConsole.WriteLine($"{_prompt}{separator} [green]{resultText}[/]");
        return _choices[result].Value!;
    }
}
