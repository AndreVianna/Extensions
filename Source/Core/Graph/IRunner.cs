namespace DotNetToolbox.Graph;

public interface IRunner<out TContext>
    where TContext : IMap {
    public string WorkflowId { get; }
    public uint Id { get; }
    public bool IsRunning { get; }
    public DateTimeOffset? Start { get; }
    public DateTimeOffset? End { get; }
    public TimeSpan? ElapsedTime { get; }
    public bool HasStarted { get; }
    public bool HasStopped { get; }

    Task Run(CancellationToken ct = default);

    Func<IWorkflow<TContext>, CancellationToken, Task>? OnStartingWorkflow { set; }
    Func<IWorkflow<TContext>, INode, CancellationToken, Task<bool>>? OnExecutingNode { set; }
    Func<IWorkflow<TContext>, INode, INode?, CancellationToken, Task<bool>>? OnNodeExecuted { set; }
    Func<IWorkflow<TContext>, CancellationToken, Task>? OnWorkflowEnded { set; }
}
