namespace DotNetToolbox.ConsoleApplication.Nodes;

public interface ICommand : IHasParent, IHasChildren {
    Task<IValidationResult> Execute(IReadOnlyList<string> args, CancellationToken ct = default);
}
