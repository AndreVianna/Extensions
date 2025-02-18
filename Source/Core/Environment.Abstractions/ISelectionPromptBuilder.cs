namespace DotNetToolbox.Environment;

public interface ISelectionPromptBuilder :
    ISelectionPromptBuilder<string, uint>;

public interface ISelectionPromptBuilder<TValue> :
    ISelectionPromptBuilder<TValue, object>;

public interface ISelectionPromptBuilder<TValue, in TKey>
    where TKey : notnull {
    ISelectionPromptBuilder<TValue, TKey> DisplayAs(Func<TValue, string> displayAs);
    ISelectionPromptBuilder<TValue, TKey> SetAsDefault(TKey defaultKey);
    ISelectionPromptBuilder<TValue, TKey> AddDefaultChoice(TKey key, string text, ChoicePosition position = default);
    ISelectionPromptBuilder<TValue, TKey> AddDefaultChoice(TKey key, TValue choice, ChoicePosition position = default);
    ISelectionPromptBuilder<TValue, TKey> AddDefaultChoice(TValue choice, ChoicePosition position = default);
    ISelectionPromptBuilder<TValue, TKey> AddChoice(TKey key, string text, ChoicePosition position = default);
    ISelectionPromptBuilder<TValue, TKey> AddChoice(TKey key, TValue choice, ChoicePosition position = default);
    ISelectionPromptBuilder<TValue, TKey> AddChoice(TValue choice, ChoicePosition position = default);
    ISelectionPromptBuilder<TValue, TKey> AddChoices(IEnumerable<TValue> choices, ChoicePosition position = default);
    ISelectionPromptBuilder<TValue, TKey> ShowResult();
    TValue? Show();
    Task<TValue?> ShowAsync(CancellationToken ct = default);
}
