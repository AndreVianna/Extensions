namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for Console functionality.")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global - Used for externally.
public class ConsoleOutput
    : IHasDefault<ConsoleOutput>
    , IOutput {
    public static ConsoleOutput Default { get; } = new();

    public virtual void ClearScreen() => Console.Clear();

    public virtual TextWriter Writer => Console.Out;
    public virtual TextWriter ErrorWriter => Console.Error;

    public virtual Encoding Encoding {
        get => Console.OutputEncoding;
        set => Console.OutputEncoding = value;
    }

    public virtual ConsoleColor BackgroundColor {
        get => Console.BackgroundColor;
        set => Console.BackgroundColor = value;
    }

    public virtual ConsoleColor ForegroundColor {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }
    public virtual void ResetColor() => Console.ResetColor();

    public virtual string NewLine => System.Environment.NewLine;

    public virtual string Prompt { get; set; } = "> ";
    public virtual void WritePrompt() => Writer.Write(Prompt);

    #region Write

    public virtual void Write(bool value)
        => Writer.Write(value);
    public virtual void Write(ulong value)
        => Writer.Write(value);
    public virtual void Write(uint value)
        => Writer.Write(value);
    public virtual void Write(long value)
        => Writer.Write(value);
    public virtual void Write(int value)
        => Writer.Write(value);
    public virtual void Write(float value)
        => Writer.Write(value);
    public virtual void Write(double value)
        => Writer.Write(value);
    public virtual void Write(decimal value)
        => Writer.Write(value);
    public virtual void Write(char value)
        => Writer.Write(value);
    public virtual void Write(string value)
        => Writer.Write(value);
    public virtual void Write([Syntax(Syntax.CompositeFormat)] string format, params IEnumerable<object?> args)
        => Writer.Write(format, args);
    public virtual void Write(object? value)
        => Writer.Write(value);
    public virtual void Write(StringBuilder? builder)
        => Writer.Write(builder?.ToString() ?? string.Empty);
    public virtual void Write(char[]? buffer)
        => Writer.Write(buffer);
    public virtual void Write(char[] buffer, int index, int count)
        => Writer.Write(buffer, index, count);

    #endregion

    #region WriteLine

    public virtual void WriteLine()
        => Writer.WriteLine();
    public virtual void WriteLine(bool value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(ulong value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(uint value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(long value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(int value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(float value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(double value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(decimal value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(char value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(string value)
        => Writer.WriteLine(value);
    public virtual void WriteLine([Syntax(Syntax.CompositeFormat)] string format, params IEnumerable<object?> args)
        => Writer.WriteLine(format, args);
    public virtual void WriteLine(object? value)
        => Writer.WriteLine(value);
    public virtual void WriteLine(StringBuilder? builder)
        => Writer.WriteLine(builder?.ToString() ?? string.Empty);
    public virtual void WriteLine(char[]? buffer)
        => Writer.WriteLine(buffer);
    public virtual void WriteLine(char[] buffer, int index, int count)
        => Writer.WriteLine(buffer, index, count);

    #endregion

    #region EndLineThenWrite

    public virtual void EndLineThenWrite(bool value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(uint value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(ulong value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(int value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(long value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(float value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(double value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(decimal value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(char value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(string value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite([Syntax(Syntax.CompositeFormat)] string format, params object?[] args) {
        WriteLine();
        Write(format, args);
    }
    public virtual void EndLineThenWrite(object? value) {
        WriteLine();
        Write(value);
    }
    public virtual void EndLineThenWrite(StringBuilder? builder) {
        WriteLine();
        Write(builder);
    }
    public virtual void EndLineThenWrite(char[]? buffer) {
        WriteLine();
        Write(buffer);
    }
    public virtual void EndLineThenWrite(char[] buffer, int index, int count) {
        WriteLine();
        Write(buffer, index, count);
    }

    #endregion

    #region EndLineThenWriteLine

    public virtual void EndLineThenWriteLine(bool value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(uint value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(ulong value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(int value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(long value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(float value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(double value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(decimal value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(char value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(string value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine([Syntax(Syntax.CompositeFormat)] string format, params object?[] args) {
        WriteLine();
        WriteLine(format, args);
    }

    public virtual void EndLineThenWriteLine(object? value) {
        WriteLine();
        WriteLine(value);
    }

    public virtual void EndLineThenWriteLine(StringBuilder? builder) {
        WriteLine();
        WriteLine(builder);
    }

    public virtual void EndLineThenWriteLine(char[]? buffer) {
        WriteLine();
        WriteLine(buffer);
    }

    public virtual void EndLineThenWriteLine(char[] buffer, int index, int count) {
        WriteLine();
        WriteLine(buffer, index, count);
    }

    #endregion
}
