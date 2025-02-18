using ValidationResult = Spectre.Console.ValidationResult;

namespace DotNetToolbox.Environment;

public class TextPromptBuilder(string prompt, string defaultValue)
    : ITextPromptBuilder {
    private uint _maxLines;
    private Func<string, Result>? _validator;

    public TextPromptBuilder(string prompt)
        : this(prompt, string.Empty) {
    }

    public TextPromptBuilder()
        : this(string.Empty, string.Empty) {
    }

    public TextPromptBuilder AddValidation(Func<string, Result> validate) {
        var oldValidator = _validator;
        _validator = value => {
            var result = oldValidator?.Invoke(value) ?? Result.Success();
            result += validate(value);
            return result;
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

    private Func<string, ValidationResult>? BuildValidator()
        => _validator is null ? null : value => {
            var result = _validator(value);
            if (result.IsSuccess) return ValidationResult.Success();
            if (result.Errors.Count == 1) return ValidationResult.Error($"[red]{result.Errors[0].Message}[/]");
            var errors = new StringBuilder();
            errors.AppendLine("[red]The input value is invalid.[/]");
            foreach (var item in result.Errors)
                errors.AppendLine($"[red] - {item.Message}[/]");
            return ValidationResult.Error(errors.ToString());
        };
}
