
namespace DotNetToolbox.Graph;

public interface IWorkflow : IWorkflow<Map>;

public interface IWorkflow<out TContext>
    where TContext : IMap {
    string Id { get; }
    TContext Context { get; }
    INode StartNode { get; }

    Result Validate();
    Task Run(CancellationToken ct = default);
}
