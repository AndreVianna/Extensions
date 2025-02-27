using System.Diagnostics.CodeAnalysis;

namespace DotNetToolbox.ConsoleApplication.TestDoubles;

internal sealed class TestOutput : IOutput {
    public string Prompt { get; set; } = "> ";
    public override string ToString() => string.Join(NewLine, Lines);
    public List<string> Lines { get; } = [];

    public TextWriter ErrorWriter => throw new NotImplementedException();
    public TextWriter Writer => throw new NotImplementedException();

    public Encoding Encoding {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public ConsoleColor ForegroundColor {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
    public ConsoleColor BackgroundColor {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public void ClearScreen() {
        Lines.Clear();
        Lines.Add(string.Empty);
    }

    public void ResetColor() => throw new NotImplementedException();
    public void Write(long value) => throw new NotImplementedException();

    public void Write(string value) {
        var lines = value.Split(System.Environment.NewLine);
        if (lines.Length == 0) return;
        if (Lines.Count == 0) Lines.Add(lines[0]);
        else Lines[^1] += lines[0];
        Lines.AddRange(lines.Skip(1));
    }

    public void Write(string format, params IEnumerable<object?> args) => throw new NotImplementedException();

    public void Write(object? value) => throw new NotImplementedException();

    public void Write(StringBuilder? builder) => throw new NotImplementedException();

    public void Write(uint value) => throw new NotImplementedException();

    public void Write(ulong value) => throw new NotImplementedException();

    public void WriteLine() => throw new NotImplementedException();

    public void WriteLine(bool value) => throw new NotImplementedException();

    public void WriteLine(char value) => throw new NotImplementedException();

    public void WriteLine(char[] buffer, int index, int count) => throw new NotImplementedException();

    public void WriteLine(char[]? buffer) => throw new NotImplementedException();

    public void WriteLine(decimal value) => throw new NotImplementedException();

    public void WriteLine(double value) => throw new NotImplementedException();

    public void WriteLine(float value) => throw new NotImplementedException();

    public void WriteLine(int value) => throw new NotImplementedException();

    public void WriteLine(uint value) => throw new NotImplementedException();

    public void WriteLine(long value) => throw new NotImplementedException();

    public void WriteLine(ulong value) => throw new NotImplementedException();

    public void Write([StringSyntax("CompositeFormat")] string format, params object?[] args)
        => Lines[^1] += string.Format(format, args);

    public void WriteLine(string value) {
        Write(value);
        WriteLine();
    }

    public void WriteLine(string format, params IEnumerable<object?> args) => throw new NotImplementedException();

    public void WriteLine([StringSyntax("CompositeFormat")] string format, params object?[] args) {
        Write(format, args);
        WriteLine();
    }
    public void WriteLine(object? value) {
        Write("{0}", value);
        WriteLine();
    }

    public void WriteLine(StringBuilder? builder) => throw new NotImplementedException();

    public void EndLineThenWrite(bool value) => throw new NotImplementedException();

    public void EndLineThenWrite(char value) => throw new NotImplementedException();

    public void EndLineThenWrite(char[] buffer, int index, int count) => throw new NotImplementedException();

    public void EndLineThenWrite(char[]? buffer) => throw new NotImplementedException();

    public void EndLineThenWrite(decimal value) => throw new NotImplementedException();

    public void EndLineThenWrite(double value) => throw new NotImplementedException();

    public void EndLineThenWrite(float value) => throw new NotImplementedException();

    public void EndLineThenWrite(int value) => throw new NotImplementedException();

    public void EndLineThenWrite(uint value) => throw new NotImplementedException();

    public void EndLineThenWrite(long value) => throw new NotImplementedException();

    public void EndLineThenWrite(ulong value) => throw new NotImplementedException();

    public void EndLineThenWrite(string value) => throw new NotImplementedException();

    public void EndLineThenWrite(string format, params object?[] args) => throw new NotImplementedException();

    public void EndLineThenWrite(object? value) => throw new NotImplementedException();

    public void EndLineThenWrite(StringBuilder? builder) => throw new NotImplementedException();

    public void EndLineThenWriteLine(bool value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(char value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(char[] buffer, int index, int count) => throw new NotImplementedException();

    public void EndLineThenWriteLine(char[]? buffer) => throw new NotImplementedException();

    public void EndLineThenWriteLine(decimal value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(double value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(float value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(int value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(uint value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(long value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(ulong value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(string value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(string format, params object?[] args) => throw new NotImplementedException();

    public void EndLineThenWriteLine(object? value) => throw new NotImplementedException();

    public void EndLineThenWriteLine(StringBuilder? builder) => throw new NotImplementedException();

    public void WritePrompt() {
        if (Lines.Count == 0) Lines.Add(Prompt);
        else Lines[^1] += Prompt;
    }

    public void Write(bool value) => throw new NotImplementedException();

    public void Write(char value) => throw new NotImplementedException();

    public void Write(char[] buffer, int index, int count) => throw new NotImplementedException();

    public void Write(char[]? buffer) => throw new NotImplementedException();

    public void Write(decimal value) => throw new NotImplementedException();

    public void Write(double value) => throw new NotImplementedException();

    public void Write(float value) => throw new NotImplementedException();

    public void Write(int value) => throw new NotImplementedException();

    public string NewLine => System.Environment.NewLine;
}
