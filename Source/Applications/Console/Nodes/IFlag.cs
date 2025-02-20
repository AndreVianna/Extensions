namespace DotNetToolbox.ConsoleApplication.Nodes;

public interface IFlag : IArgument {
    Task<IValidationResult> Read(IMap context, CancellationToken ct = default);
}
