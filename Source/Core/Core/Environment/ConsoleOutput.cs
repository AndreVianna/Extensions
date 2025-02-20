using Spectre.Console.Rendering;

namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for Console functionality.")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global - Used for externally.
public class ConsoleOutput
    : HasDefault<ConsoleOutput>,
      IOutput {
    public virtual string Prompt { get; set; } = "> ";

    public virtual Encoding Encoding {
        get => Console.OutputEncoding;
        set => Console.OutputEncoding = value;
    }

    public virtual ConsoleColor ForegroundColor {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }
    public virtual ConsoleColor BackgroundColor {
        get => Console.BackgroundColor;
        set => Console.BackgroundColor = value;
    }

    public virtual TextWriter Writer => Console.Out;
    public virtual TextWriter ErrorWriter => Console.Error;

    public virtual void ResetColor() => Console.ResetColor();
    public virtual void ClearScreen() => Console.Clear();

    public virtual void Write(bool value)
        => AnsiConsole.Write(value);
    public virtual void Write(ulong value)
        => AnsiConsole.Write(value);
    public virtual void Write(uint value)
        => AnsiConsole.Write(value);
    public virtual void Write(long value)
        => AnsiConsole.Write(value);
    public virtual void Write(int value)
        => AnsiConsole.Write(value);
    public virtual void Write(float value)
        => AnsiConsole.Write(value);
    public virtual void Write(double value)
        => AnsiConsole.Write(value);
    public virtual void Write(decimal value)
        => AnsiConsole.Write(value);
    public virtual void Write(char value)
        => AnsiConsole.Write(value);
    public virtual void Write(string value)
        => AnsiConsole.Markup(value);
    public virtual void Write([Syntax(Syntax.CompositeFormat)] string format, params object?[] args)
        => AnsiConsole.Markup(format, args!);
    public virtual void Write(object? value) {
        if (value is IRenderable text) AnsiConsole.Write(text);
        else Write($"{value}");
    }
    public virtual void Write(StringBuilder? builder)
        => AnsiConsole.Markup(builder?.ToString() ?? string.Empty);
    public virtual void Write(char[]? buffer)
        => AnsiConsole.Markup(new(buffer));
    public virtual void Write(char[] buffer, int index, int count)
        => AnsiConsole.Markup(new(buffer), index, count);

    public virtual void WritePrompt()
        => AnsiConsole.Markup(Prompt);
    public virtual string NewLine => System.Environment.NewLine;

    public virtual void WriteLine()
        => AnsiConsole.WriteLine();
    public virtual void WriteLine(bool value)
        => AnsiConsole.WriteLine(value);
    public virtual void WriteLine(ulong value)
        => AnsiConsole.WriteLine(value);
    public virtual void WriteLine(uint value)
        => AnsiConsole.WriteLine(value);
    public virtual void WriteLine(long value)
        => AnsiConsole.WriteLine(value);
    public virtual void WriteLine(int value)
        => AnsiConsole.WriteLine(value);
    public virtual void WriteLine(float value)
        => AnsiConsole.WriteLine(value);
    public virtual void WriteLine(double value)
        => AnsiConsole.WriteLine(value);
    public virtual void WriteLine(decimal value)
        => AnsiConsole.WriteLine(value);
    public virtual void WriteLine(char value)
        => AnsiConsole.WriteLine(value);
    public virtual void WriteLine(string value)
        => AnsiConsole.MarkupLine(value);
    public virtual void WriteLine([Syntax(Syntax.CompositeFormat)] string format, params object?[] args)
        => AnsiConsole.MarkupLine(format, args!);
    public virtual void WriteLine(object? value) {
        if (value is IRenderable widget) AnsiConsole.Write(widget);
        else Write($"{value}");
    }
    public virtual void WriteLine(StringBuilder? builder)
        => AnsiConsole.MarkupLine(builder?.ToString() ?? string.Empty);
    public virtual void WriteLine(char[]? buffer)
        => AnsiConsole.MarkupLine(new(buffer));
    public virtual void WriteLine(char[] buffer, int index, int count)
        => AnsiConsole.MarkupLine(new(buffer), index, count);

    public virtual void WriteOnANewLine(bool value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(uint value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(ulong value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(int value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(long value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(float value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(double value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(decimal value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(char value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(string value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine([Syntax(Syntax.CompositeFormat)] string format, params object?[] args) {
        WriteLine();
        Write(format, args);
    }
    public virtual void WriteOnANewLine(object? value) {
        WriteLine();
        Write(value);
    }
    public virtual void WriteOnANewLine(StringBuilder? builder) {
        WriteLine();
        Write(builder);
    }
    public virtual void WriteOnANewLine(char[]? buffer) {
        WriteLine();
        Write(buffer);
    }
    public virtual void WriteOnANewLine(char[] buffer, int index, int count) {
        WriteLine();
        Write(buffer, index, count);
    }

#if DEBUG
    private const ExceptionFormats _exceptionFormat = ExceptionFormats.ShowLinks;
#endif

    public virtual void WriteError(Exception exception) {
        WriteLine();
#if DEBUG
        AnsiConsole.WriteException(exception, _exceptionFormat);
#else
        WriteLine($"[bold red]An error has occurred! {exception.Message}[/]");
#endif
        WriteLine();
    }

    public virtual void WriteError(Exception exception, string message) {
        WriteLine();
        WriteLine($"[bold red]An error has occurred!  {IsNotNullOrWhiteSpace(message)}[/]");
#if DEBUG
        AnsiConsole.WriteException(exception, _exceptionFormat);
#endif
        WriteLine();
    }

    public virtual void WriteError(string message) {
        WriteLine();
        WriteLine($"[bold red]An error has occurred! {IsNotNullOrWhiteSpace(message)}[/]");
        WriteLine();
    }
}
