namespace DotNetToolbox.Graph;

public interface IRunner<out TContext>
    where TContext : IMap {
    string WorkflowId { get; }
    uint Id { get; }
    bool IsRunning { get; }
    DateTimeOffset? Start { get; }
    DateTimeOffset? End { get; }
    TimeSpan? ElapsedTime { get; }
    bool HasStarted { get; }
    bool HasStopped { get; }

    Task Run(CancellationToken ct = default);

    Func<IWorkflow<TContext>, CancellationToken, Task>? OnStartingWorkflow { set; }
    Func<IWorkflow<TContext>, INode, CancellationToken, Task<bool>>? OnExecutingNode { set; }
    Func<IWorkflow<TContext>, INode, INode?, CancellationToken, Task<bool>>? OnNodeExecuted { set; }
    Func<IWorkflow<TContext>, CancellationToken, Task>? OnWorkflowEnded { set; }
}
