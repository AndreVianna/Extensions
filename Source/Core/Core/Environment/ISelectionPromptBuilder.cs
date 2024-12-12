namespace DotNetToolbox.Environment;

public interface ISelectionPromptBuilder :
    ISelectionPromptBuilder<string, uint>;

public interface ISelectionPromptBuilder<TValue> :
    ISelectionPromptBuilder<TValue, object>;

public interface ISelectionPromptBuilder<TValue, TKey>
    where TKey : notnull {
    SelectionPromptBuilder<TValue, TKey> DisplayAs(Func<TValue, string> displayAs);
    SelectionPromptBuilder<TValue, TKey> SetAsDefault(TKey defaultKey);
    SelectionPromptBuilder<TValue, TKey> AddDefaultChoice(TKey key, string text, ChoicePosition position = default);
    SelectionPromptBuilder<TValue, TKey> AddDefaultChoice(TKey key, TValue choice, ChoicePosition position = default);
    SelectionPromptBuilder<TValue, TKey> AddDefaultChoice(TValue choice, ChoicePosition position = default);
    SelectionPromptBuilder<TValue, TKey> AddChoice(TKey key, string text, ChoicePosition position = default);
    SelectionPromptBuilder<TValue, TKey> AddChoice(TKey key, TValue choice, ChoicePosition position = default);
    SelectionPromptBuilder<TValue, TKey> AddChoice(TValue choice, ChoicePosition position = default);
    SelectionPromptBuilder<TValue, TKey> AddChoices(IEnumerable<TValue> choices, ChoicePosition position = default);
    SelectionPromptBuilder<TValue, TKey> ShowResult();
    TValue? Show();
    Task<TValue?> ShowAsync(CancellationToken ct = default);
}
