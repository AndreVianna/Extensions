namespace DotNetToolbox.Graph.Nodes;

public class IfNode : IfNode<IfNode> {
    private readonly Func<IMap, CancellationToken, Task<bool>> _predicate;

    private IfNode(string? tag, string? name, Func<IMap, CancellationToken, Task<bool>>? predicate, IServiceProvider services)
        : base(tag, services) {
        Name = name ?? Name;
        Label = name ?? "if";
        _predicate = predicate!;
    }
    public IfNode(string? name, IServiceProvider services)
        : this(null, name, null, services) {
    }
    public IfNode(string? tag, string? name, IServiceProvider services)
        : this(tag, name, null, services) {
    }
    public IfNode(Func<IMap, CancellationToken, Task<bool>> predicate, IServiceProvider services)
        : this(null, null, predicate, services) {
    }
    public IfNode(string? tag, Func<IMap, CancellationToken, Task<bool>> predicate, IServiceProvider services)
        : this(tag, null, predicate, services) {
    }
    public IfNode(Func<IMap, bool> predicate, IServiceProvider services)
        : this(null, null, (ctx, ct) => Task.Run(() => predicate(ctx), ct), services) {
    }
    public IfNode(string? tag, Func<IMap, bool> predicate, IServiceProvider services)
        : this(tag, null, (ctx, ct) => Task.Run(() => predicate(ctx), ct), services) {
    }

    protected override Task<bool> If(IMap context, CancellationToken ct = default) => _predicate(context, ct);
}

public abstract class IfNode<TNode>(string? tag, IServiceProvider services)
    : Node<TNode>(tag, services),
      IIfNode
    where TNode : IfNode<TNode> {
    public string Name { get; set; } = typeof(TNode).Name;
    public INode? Then { get; set; }
    public INode? Else { get; set; }

    protected override IValidationResult IsValid(ISet<INode> visited) {
        var result = base.IsValid(visited);
        result.Add(Then?.Validate(visited) ?? Failure("The true node is not set.", Token?.ToSource()));
        result.Add(Else?.Validate(visited) ?? Default);
        return result;
    }

    protected sealed override Task UpdateState(IMap context, CancellationToken ct = default)
        => Task.CompletedTask;

    protected sealed override async Task<INode?> SelectPath(IMap context, CancellationToken ct = default) {
        var result = await If(context, ct);
        return result
            ? await ThenDo(context, ct)
            : await ElseDo(context, ct);
    }

    protected abstract Task<bool> If(IMap context, CancellationToken ct = default);

    private Task<INode?> ThenDo(IMap context, CancellationToken ct)
        => Then is not null ? Then.Run(context, ct) : Task.FromResult<INode?>(null);

    private Task<INode?> ElseDo(IMap context, CancellationToken ct)
        => Else is not null ? Else.Run(context, ct) : Task.FromResult<INode?>(null);

    public sealed override void ConnectTo(INode? next) {
        if (Then is null) Then = next;
        else Then.ConnectTo(next);
        if (Else is null) Else = next;
        else Else.ConnectTo(next);
    }
}
