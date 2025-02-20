namespace DotNetToolbox.ConsoleApplication.Extensions;

public class TextPromptBuilder(string prompt, string defaultValue)
    : ITextPromptBuilder {
    private uint _maxLines;
    private Func<string, IValidationResult>? _validator;

    public TextPromptBuilder(string prompt)
        : this(prompt, string.Empty) {
    }

    public TextPromptBuilder()
        : this(string.Empty, string.Empty) {
    }

    public TextPromptBuilder AddValidation(Func<string, IValidationResult> validate) {
        var oldValidator = _validator;
        _validator = value => {
            var result = oldValidator?.Invoke(value) ?? Results.ValidationResult.Success();
            return result.Add(validate(value));
        };
        return this;
    }

    public TextPromptBuilder MaximumNumberOfLines(uint lines) {
        _maxLines = lines;
        return this;
    }

    public string Show() => ShowAsync().GetAwaiter().GetResult();

    public async Task<string> ShowAsync(CancellationToken ct = default) {
        var firstLine = new Markup("[yellow]>[/]");
        var otherLines = new Markup("[yellow]|[/]");
        var editor = new TextEditor {
            MaximumNumberOfLines = _maxLines,
            PromptPrefix = new PromptPrefix(firstLine, otherLines),
            Text = defaultValue,
            Validator = BuildValidator(),
        };

        var formattedPrompt = FormatPrompt(editor);
        var result = string.Empty;
        while (editor.State == TextEditorState.Active) {
            AnsiConsole.WriteLine(formattedPrompt);
            result = await editor.ReadText(ct) ?? result;
            if (editor.State is not TextEditorState.Invalid) continue;
            AnsiConsole.WriteLine(editor.ErrorMessage);
            AnsiConsole.WriteLine("[yellow]Please try again.[/]");
            AnsiConsole.WriteLine();
            editor.ResetState();
        }

        return result;
    }

    private string FormatPrompt(TextEditor editor) {
        var formatedPrompt = $"[teal]{prompt}[/]";
        if (_maxLines != 1) {
            formatedPrompt += $"{System.Environment.NewLine}[gray]Press ENTER to insert a new line, CTRL+ENTER to submit, and ESCAPE to cancel.[/]";
            editor.KeyBindings.Add<NewLineCommand>(ConsoleKey.Enter);
            editor.KeyBindings.Add<SubmitCommand>(ConsoleKey.Enter, ConsoleModifiers.Control);
        }
        else {
            formatedPrompt += $"{System.Environment.NewLine}[gray]Press ENTER to submit and ESCAPE to cancel.[/]";
        }

        return formatedPrompt;
    }

    private Func<string, Spectre.Console.ValidationResult>? BuildValidator()
        => _validator is null ? null : value => {
            var result = _validator(value);
            if (result.IsSuccess) return Spectre.Console.ValidationResult.Success();
            if (result.Errors.Count == 1) return Spectre.Console.ValidationResult.Error($"[red]{result.Errors.First().Message}[/]");
            var errors = new StringBuilder();
            errors.AppendLine("[red]The input value is invalid.[/]");
            foreach (var item in result.Errors)
                errors.AppendLine($"[red] - {item.Message}[/]");
            return Spectre.Console.ValidationResult.Error(errors.ToString());
        };
}
