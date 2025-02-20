namespace DotNetToolbox.Environment;

public interface IPromptBuilder
    : ILinePromptBuilder<string>;

public interface ILinePromptBuilder<TValue> {
    ILinePromptBuilder<TValue> UseMask(char? maskChar);
    ILinePromptBuilder<TValue> ConvertWith(Func<TValue, string> converter);
    ILinePromptBuilder<TValue> AddChoices(IEnumerable<TValue> choices);
    ILinePromptBuilder<TValue> AddChoices(TValue choice, params TValue[] otherChoices);
    ILinePromptBuilder<TValue> ShowOptionalFlag();
    ILinePromptBuilder<TValue> AddValidation(Func<TValue, IValidationResult> validate);
    TValue Show();
    Task<TValue> ShowAsync(CancellationToken ct = default);
}
