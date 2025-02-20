namespace DotNetToolbox.ConsoleApplication.Nodes;

public interface IOption : IArgument {
    Task<IValidationResult> Read(string? value, IMap context, CancellationToken ct = default);
}
