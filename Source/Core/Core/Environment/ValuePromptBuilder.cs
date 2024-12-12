namespace DotNetToolbox.Environment;

public class ValuePromptBuilder<TValue>(string prompt)
    : ILinePromptBuilder<TValue> {
    private readonly List<TValue> _choices = [];
    private string _prompt = IsNotNullOrWhiteSpace(prompt);
    private bool _isRequired = true;
    private Func<TValue, Result>? _validator;
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

    public ValuePromptBuilder<TValue> ConvertWith(Func<TValue, string> converter) {
        _converter = converter;
        return this;
    }

    public ValuePromptBuilder<TValue> UseMask(char? maskChar) {
        _maskChar = maskChar ?? '*';
        return this;
    }

    public ValuePromptBuilder<TValue> AddValidation(Func<TValue, Result> validate) {
        var oldValidator = _validator;
        _validator = value => {
            var result = oldValidator is null ? Result.Success() : oldValidator(value);
            result += validate(value);
            return result;
        };
        return this;
    }

    public ValuePromptBuilder<TValue> ShowOptionalFlag() {
        _isRequired = false;
        return this;
    }

    public ValuePromptBuilder<TValue> AddChoices(IEnumerable<TValue> choices) {
        _choices.AddRange(choices.Distinct());
        return this;
    }

    public ValuePromptBuilder<TValue> AddChoices(TValue choice, params TValue[] otherChoices)
        => AddChoices([choice, .. otherChoices]);

    private Func<TValue, ValidationResult> BuildValidator()
        => value => {
            var result = _validator?.Invoke(value) ?? Result.Success();
            if (result.IsSuccess) return ValidationResult.Success();
            if (result.Errors.Count == 1) return ValidationResult.Error($"[red]{result.Errors[0].Message}[/]");
            var errors = new StringBuilder();
            errors.AppendLine("[red]The entry is invalid.[/]");
            foreach (var item in result.Errors)
                errors.AppendLine($"[red] - {item.Message}[/]");
            return ValidationResult.Error(errors.ToString());
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

    private Result ValidateChoices(TValue value)
        => _choices.Count == 0 || _choices.Contains(value)
               ? Result.Success()
               : Result.Invalid("Please select one of the available options");
}
