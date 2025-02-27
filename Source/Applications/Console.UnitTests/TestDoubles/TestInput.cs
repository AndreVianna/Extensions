namespace DotNetToolbox.ConsoleApplication.TestDoubles;

internal sealed class TestInput(IOutput output, params string[] inputs)
    : IInput {
    private readonly Queue<string> _inputQueue = new(inputs);

    public Encoding Encoding {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public TextReader Reader => throw new NotImplementedException();

    public int Read() => throw new NotImplementedException();

    public ConsoleKeyInfo ReadKey(bool intercept = false) => throw new NotImplementedException();

    public string ReadLine() {
        if (!_inputQueue.TryDequeue(out var input)) return string.Empty;
        output.WriteLine(input);
        return input;
    }
    public ValueTask<string?> ReadLineAsync(CancellationToken ct = default) => throw new NotImplementedException();
}
