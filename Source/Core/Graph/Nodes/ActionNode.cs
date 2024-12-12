namespace DotNetToolbox.Graph.Nodes;

public sealed class ActionNode : ActionNode<ActionNode> {
    private ActionNode(string? tag, string? name, Func<IMap, CancellationToken, Task>? execute, IServiceProvider services)
        : base(tag, services) {
        Name = name ?? Name;
        Label = name ?? "action";
        _execute = execute!;
    }
    public ActionNode(string? name, IServiceProvider services)
        : this(null, name, null, services) {
    }
    public ActionNode(string? tag, string? name, IServiceProvider services)
        : this(tag, name, null, services) {
    }
    public ActionNode(Func<IMap, CancellationToken, Task> execute, IServiceProvider services)
        : this(null, null, execute, services) {
    }
    public ActionNode(string? tag, Func<IMap, CancellationToken, Task> execute, IServiceProvider services)
        : this(tag, null, execute, services) {
        Tag = IsNotNullOrWhiteSpace(tag);
    }
    public ActionNode(Action<IMap> execute, IServiceProvider services)
        : this(null, null, (ctx, ct) => Task.Run(() => execute(ctx), ct), services) {
    }
    public ActionNode(string? tag, Action<IMap> execute, IServiceProvider services)
        : this(tag, null, (ctx, ct) => Task.Run(() => execute(ctx), ct), services) {
    }

    private readonly Func<IMap, CancellationToken, Task> _execute;

    protected override Task Execute(IMap context, CancellationToken ct = default)
        => _execute(context, ct);

    public static TNode Create<TNode>(IServiceProvider services, params object[] args)
        where TNode : ActionNode<TNode>
        => Node.Create<TNode>(services, args);
}

public abstract class ActionNode<TNode>(string? tag, IServiceProvider services)
    : Node<TNode>(tag, services),
      IActionNode
    where TNode : ActionNode<TNode> {
    public sealed override void ConnectTo(INode? next) {
        if (Next is null) Next = next;
        else Next.ConnectTo(next);
    }

    public string Name { get; set; } = typeof(TNode).Name;
    public IRetryPolicy Retry { get; set; } = services.GetService<IRetryPolicy>()
                                           ?? RetryPolicy.Default;

    protected sealed override Task<INode?> SelectPath(IMap context, CancellationToken ct = default)
        => Task.FromResult(Next);

    protected sealed override Task UpdateState(IMap context, CancellationToken ct = default)
        => Retry.Execute(Execute, context, ct);

    protected abstract Task Execute(IMap context, CancellationToken ct = default);
}
