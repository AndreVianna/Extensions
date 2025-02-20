namespace DotNetToolbox.Environment;

public interface IOutput {
    string Prompt { get; set; }
    ConsoleColor BackgroundColor { get; set; }
    Encoding Encoding { get; set; }
    TextWriter ErrorWriter { get; }
    ConsoleColor ForegroundColor { get; set; }
    TextWriter Writer { get; }

    void ClearScreen();
    void ResetColor();
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
    void Write([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args);
    void Write(object? value);
    void Write(StringBuilder? builder);
    void Write(uint value);
    void Write(ulong value);
    void WritePrompt();

    string NewLine { get; }
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
    void WriteLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args);
    void WriteLine(object? value);
    void WriteLine(StringBuilder? builder);
    void WriteOnANewLine(bool value);
    void WriteOnANewLine(char value);
    void WriteOnANewLine(char[] buffer, int index, int count);
    void WriteOnANewLine(char[]? buffer);
    void WriteOnANewLine(decimal value);
    void WriteOnANewLine(double value);
    void WriteOnANewLine(float value);
    void WriteOnANewLine(int value);
    void WriteOnANewLine(uint value);
    void WriteOnANewLine(long value);
    void WriteOnANewLine(ulong value);
    void WriteOnANewLine(string value);
    void WriteOnANewLine([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string format, params object?[] args);
    void WriteOnANewLine(object? value);
    void WriteOnANewLine(StringBuilder? builder);
    void WriteError(Exception exception);
    void WriteError(Exception exception, string message);
    void WriteError(string message);
}
