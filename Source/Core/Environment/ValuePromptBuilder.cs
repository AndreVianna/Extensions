using static DotNetToolbox.Results.ResultFactory;
using static DotNetToolbox.Results.ValidationState;

using SpectreConsoleResult = Spectre.Console.ValidationResult;

namespace DotNetToolbox.Environment;

public class ValuePromptBuilder<TValue>(string prompt)
    : ILinePromptBuilder<TValue> {
    private readonly List<TValue> _choices = [];
    private string _prompt = IsNotNullOrWhiteSpace(prompt);
    private bool _isRequired = true;
    private Func<TValue, IResult>? _validator;
    private Func<TValue, string>? _converter;
    private char? _maskChar;
    private readonly TValue? _defaultValue;

    [MemberNotNullWhen(true, nameof(_defaultValue))]
    private bool HasDefault { get; }

    public ValuePromptBuilder()
        : this(string.Empty) {
    }

    public ValuePromptBuilder(string prompt, TValue defaultValue)
        : this(prompt) {
        _defaultValue = IsNotNull(defaultValue);
        HasDefault = _defaultValue is not string text || !string.IsNullOrEmpty(text);
    }

    public ILinePromptBuilder<TValue> ConvertWith(Func<TValue, string> converter) {
        _converter = converter;
        return this;
    }

    public ILinePromptBuilder<TValue> UseMask(char? maskChar) {
        _maskChar = maskChar ?? '*';
        return this;
    }

    public ILinePromptBuilder<TValue> AddValidation(Func<TValue, IResult> validate) {
        var oldValidator = _validator;
        _validator = value => {
            var result = (Result<TValue>)(oldValidator is null ? Success(value) : oldValidator(value));
            result += (Result<TValue>)validate(value);
            return result;
        };
        return this;
    }

    public ILinePromptBuilder<TValue> ShowOptionalFlag() {
        _isRequired = false;
        return this;
    }

    public ILinePromptBuilder<TValue> AddChoices(IEnumerable<TValue> choices) {
        _choices.AddRange(choices.Distinct());
        return this;
    }

    public ILinePromptBuilder<TValue> AddChoices(TValue choice, params TValue[] otherChoices)
        => AddChoices([choice, .. otherChoices]);

    private Func<TValue, SpectreConsoleResult> BuildValidator()
        => value => {
            var result = _validator?.Invoke(value) ?? Success();
            if (result.Is(ValidationState.Success)) return SpectreConsoleResult.Success();
            if (result.Errors.Count == 1) return SpectreConsoleResult.Error($"[red]{result.Errors.First().Message}[/]");
            var errors = new StringBuilder();
            errors.AppendLine("[red]The entry is invalid.[/]");
            foreach (var item in result.Errors)
                errors.AppendLine($"[red] - {item.Message}[/]");
            return SpectreConsoleResult.Error(errors.ToString());
        };

    public TValue Show() => ShowAsync().GetAwaiter().GetResult();

    public Task<TValue> ShowAsync(CancellationToken ct = default) {
        _prompt = $"[teal]{_prompt}[/]";
        if (!_isRequired) _prompt = "[green](optional)[/] " + _prompt;
        var prompt = new TextPrompt<TValue>(_prompt);
        prompt.AllowEmpty().ChoicesStyle(new(foreground: Color.Blue));
        if (_maskChar is not null) prompt = prompt.Secret(_maskChar);
        if (HasDefault) prompt.DefaultValue(_defaultValue);
        if (_choices.Count > 0) {
            prompt.AddChoices(_choices);
            prompt.ShowChoices();
            AddValidation(ValidateChoices);
        }
        if (_converter is not null) prompt.Converter = _converter;
        if (_validator is not null) prompt.Validator = BuildValidator();

        return prompt.ShowAsync(AnsiConsole.Console, ct);
    }

    private IResult ValidateChoices(TValue value)
        => _choices.Count == 0 || _choices.Contains(value)
               ? Success()
               : Failure("Please select one of the available options");
}
