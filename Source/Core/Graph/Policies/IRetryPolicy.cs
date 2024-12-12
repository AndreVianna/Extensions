namespace DotNetToolbox.Graph.Policies;

public interface IRetryPolicy {
    IReadOnlyList<TimeSpan> Delays { get; }
    byte MaxRetries { get; }
    Task Execute(Func<IMap, CancellationToken, Task> action, IMap ctx, CancellationToken ct = default);
}
