namespace DotNetToolbox.Environment;

public interface IPromptBuilder
    : ILinePromptBuilder<string>;

public interface ILinePromptBuilder<TValue> {
    ValuePromptBuilder<TValue> UseMask(char? maskChar);
    ValuePromptBuilder<TValue> ConvertWith(Func<TValue, string> converter);
    ValuePromptBuilder<TValue> AddChoices(IEnumerable<TValue> choices);
    ValuePromptBuilder<TValue> AddChoices(TValue choice, params TValue[] otherChoices);
    ValuePromptBuilder<TValue> ShowOptionalFlag();
    ValuePromptBuilder<TValue> AddValidation(Func<TValue, Result> validate);
    TValue Show();
    Task<TValue> ShowAsync(CancellationToken ct = default);
}
