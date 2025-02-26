namespace DotNetToolbox.Environment;

public interface IInput {
    Encoding Encoding { get; set; }
    TextReader Reader { get; }

    int Read();
    ConsoleKeyInfo ReadKey(bool intercept = false);

    string? ReadLine();
    ValueTask<string?> ReadLineAsync(CancellationToken ct = default);
}
