namespace DotNetToolbox.Threading;

public interface IAwaiter {
    bool IsWaiting { get; }
    Task StartWait(CancellationToken ct);
    void StopWaiting();
}
