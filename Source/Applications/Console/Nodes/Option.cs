namespace DotNetToolbox.ConsoleApplication.Nodes;

public sealed class Option(IHasChildren parent, string name, Action<Option>? configure = null)
        : Option<Option>(parent, name, configure);

public abstract class Option<TOption>(IHasChildren parent, string name, Action<TOption>? configure = null)
    : Node<TOption>(parent, name, configure), IOption
    where TOption : Option<TOption> {
    Task<Result> IOption.Read(string? value, IMap context, CancellationToken ct) {
        context[Name] = value switch {
            null or "null" or "default" => null!,
            ['"', .. var text, '"'] => text,
            _ => value,
        };

        return Execute(ct);
    }

    protected virtual Task<Result> Execute(CancellationToken ct = default) => Task.FromResult(Success());
}
