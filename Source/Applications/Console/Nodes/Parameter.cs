namespace DotNetToolbox.ConsoleApplication.Nodes;

public sealed class Parameter(IHasChildren parent, string name, Action<Parameter>? configure = null)
        : Parameter<Parameter>(parent, name, configure);

public abstract class Parameter<TParameter>(IHasChildren parent, string name, Action<TParameter>? configure = null)
    : Node<TParameter>(parent, name, configure),
      IParameter
    where TParameter : Parameter<TParameter> {
    private string? _defaultValue;

    public string? DefaultValue {
        get => _defaultValue;
        set {
            _defaultValue = value;
            Parent.Context[Name] = _defaultValue!;
        }
    }

    public int Order { get; } = parent.Children.OfType<IParameter>().Count();
    public bool IsRequired => DefaultValue is null;
    public bool IsSet { get; private set; }

    Task<Result> IParameter.Read(string? value, IMap context, CancellationToken ct) {
        context[Name] = value switch {
            null or "default" => DefaultValue!,
            "null" => null!,
            ['"', .. var text, '"'] => text,
            _ => value,
        };
        IsSet = true;
        return Execute(ct);
    }

    protected virtual Task<Result> Execute(CancellationToken ct = default) => Task.FromResult(Success());
}
