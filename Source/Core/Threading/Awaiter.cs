namespace DotNetToolbox.Threading;

public class Awaiter(int timeoutInMilliseconds = 5000, int delayInMilliseconds = 100, ILogger<Awaiter>? logger = null)
    : Awaiter<Awaiter>(timeoutInMilliseconds, delayInMilliseconds, logger);

public abstract class Awaiter<TSelf>(int timeoutInMilliseconds = 5000, int delayInMilliseconds = 100, ILogger<TSelf>? logger = null)
    : IAwaiter
    where TSelf : Awaiter<TSelf> {
    private bool _stopWaiting;
    public bool IsWaiting => _stopwatch.IsRunning;
    public void StopWaiting() => _stopwatch.Stop();

    protected ILogger<TSelf> Logger { get; } = logger ?? NullLogger<TSelf>.Instance;
    private readonly Stopwatch _stopwatch = new();

    public async Task StartWait(CancellationToken ct) {
        try {
            if (_stopwatch.IsRunning) return;
            _stopWaiting = false;
            _stopwatch.Start();
            Logger.LogDebug("Waiting...");
            while (_stopwatch.IsRunning && _stopwatch.ElapsedMilliseconds < timeoutInMilliseconds)
                await OnClockTick(ct);
            if (!_stopWaiting) throw new TimeoutException();
        }
        catch (TimeoutException) {
            Logger.LogError("Waiting timed out!");
            throw;
        }
        catch (OperationCanceledException) {
            Logger.LogWarning("Waiting cancelled.");
            throw;
        }
        finally {
            _stopwatch.Reset();
            Logger.LogDebug("Stopped waiting.");
        }
    }

    public virtual Task OnClockTick(CancellationToken ct) {
        Logger.LogTrace("Clock tick.");
        return Task.Delay(delayInMilliseconds, ct);
    }
}
