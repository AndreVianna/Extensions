namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for Console functionality.")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global - Used for externally.
public class ConsoleInput
    : IHasDefault<ConsoleInput>
    , IInput {
    public static ConsoleInput Default { get; } = new();

    public virtual Encoding Encoding {
        get => Console.InputEncoding;
        set => Console.InputEncoding = value;
    }

    public virtual TextReader Reader => Console.In;

    public virtual int Read() => Reader.Read();
    public virtual ConsoleKeyInfo ReadKey(bool intercept = false) => Console.ReadKey(intercept);
    public virtual string? ReadLine()
        => Reader.ReadLine();
    public virtual ValueTask<string?> ReadLineAsync(CancellationToken ct = default)
        => Reader.ReadLineAsync(ct);
}
