namespace DotNetToolbox.Environment;

public interface IOutput {
    void ClearScreen();

    TextWriter ErrorWriter { get; }
    TextWriter Writer { get; }

    Encoding Encoding { get; set; }

    ConsoleColor BackgroundColor { get; set; }
    ConsoleColor ForegroundColor { get; set; }
    void ResetColor();

    string NewLine { get; }

    string Prompt { get; set; }
    void WritePrompt();

    #region Write

    void Write(bool value);
    void Write(char value);
    void Write(char[] buffer, int index, int count);
    void Write(char[]? buffer);
    void Write(decimal value);
    void Write(double value);
    void Write(float value);
    void Write(int value);
    void Write(long value);
    void Write(string value);
    void Write([Syntax(Syntax.CompositeFormat)] string format, params IEnumerable<object?> args);
    void Write(object? value);
    void Write(StringBuilder? builder);
    void Write(uint value);
    void Write(ulong value);

    #endregion

    #region WriteLine

    void WriteLine();
    void WriteLine(bool value);
    void WriteLine(char value);
    void WriteLine(char[] buffer, int index, int count);
    void WriteLine(char[]? buffer);
    void WriteLine(decimal value);
    void WriteLine(double value);
    void WriteLine(float value);
    void WriteLine(int value);
    void WriteLine(uint value);
    void WriteLine(long value);
    void WriteLine(ulong value);
    void WriteLine(string value);
    void WriteLine([Syntax(Syntax.CompositeFormat)] string format, params IEnumerable<object?> args);
    void WriteLine(object? value);
    void WriteLine(StringBuilder? builder);

    #endregion

    #region EndLineThenWrite

    void EndLineThenWrite(bool value);
    void EndLineThenWrite(char value);
    void EndLineThenWrite(char[] buffer, int index, int count);
    void EndLineThenWrite(char[]? buffer);
    void EndLineThenWrite(decimal value);
    void EndLineThenWrite(double value);
    void EndLineThenWrite(float value);
    void EndLineThenWrite(int value);
    void EndLineThenWrite(uint value);
    void EndLineThenWrite(long value);
    void EndLineThenWrite(ulong value);
    void EndLineThenWrite(string value);
    void EndLineThenWrite([Syntax(Syntax.CompositeFormat)] string format, params object?[] args);
    void EndLineThenWrite(object? value);
    void EndLineThenWrite(StringBuilder? builder);

    #endregion

    #region EndLineThenWriteLine

    void EndLineThenWriteLine(bool value);
    void EndLineThenWriteLine(char value);
    void EndLineThenWriteLine(char[] buffer, int index, int count);
    void EndLineThenWriteLine(char[]? buffer);
    void EndLineThenWriteLine(decimal value);
    void EndLineThenWriteLine(double value);
    void EndLineThenWriteLine(float value);
    void EndLineThenWriteLine(int value);
    void EndLineThenWriteLine(uint value);
    void EndLineThenWriteLine(long value);
    void EndLineThenWriteLine(ulong value);
    void EndLineThenWriteLine(string value);
    void EndLineThenWriteLine([Syntax(Syntax.CompositeFormat)] string format, params object?[] args);
    void EndLineThenWriteLine(object? value);
    void EndLineThenWriteLine(StringBuilder? builder);

    #endregion
}
