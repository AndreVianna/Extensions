namespace DotNetToolbox.ConsoleApplication.Nodes;

public sealed class Flag(IHasChildren parent, string name, Action<Flag>? configure = null, Func<Flag, CancellationToken, Task<Result>>? execute = null)
    : Flag<Flag>(parent, name, configure, execute);

public abstract class Flag<TFlag>(IHasChildren parent, string name, Action<TFlag>? configure = null, Func<TFlag, CancellationToken, Task<Result>>? execute = null)
    : Node<TFlag>(parent, name, configure), IFlag
    where TFlag : Flag<TFlag> {
    Task<Result> IFlag.Read(IMap context, CancellationToken ct) {
        context[Name] = bool.TrueString;
        return Execute(ct);
    }

    protected virtual Task<Result> Execute(CancellationToken ct = default)
        => execute?.Invoke((TFlag)this, ct) ?? Task.FromResult(Success());
}
