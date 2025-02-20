namespace DotNetToolbox.ConsoleApplication.Nodes;

public sealed class Flag(IHasChildren parent, string name, Action<Flag>? configure = null, Func<Flag, CancellationToken, Task<IValidationResult>>? execute = null)
    : Flag<Flag>(parent, name, configure, execute);

public abstract class Flag<TFlag>(IHasChildren parent, string name, Action<TFlag>? configure = null, Func<TFlag, CancellationToken, Task<IValidationResult>>? execute = null)
    : Node<TFlag>(parent, name, configure), IFlag
    where TFlag : Flag<TFlag> {
    Task<IValidationResult> IFlag.Read(IMap context, CancellationToken ct) {
        context[Name] = bool.TrueString;
        return Execute(ct);
    }

    protected virtual Task<IValidationResult> Execute(CancellationToken ct = default)
        => execute?.Invoke((TFlag)this, ct) ?? Task.FromResult(Success());
}
