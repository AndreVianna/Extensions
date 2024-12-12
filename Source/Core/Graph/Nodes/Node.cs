namespace DotNetToolbox.Graph.Nodes;

public static class Node {
    [return: NotNull]
    public static TNode Create<TNode>(IServiceProvider services, params object[] args)
        where TNode : Node<TNode>
        => InstanceFactory.Create<TNode>(services, args);
    public static TNode Create<TNode>(string tag, IServiceProvider services, params object[] args)
        where TNode : Node<TNode>
        => InstanceFactory.Create<TNode>(services, [IsNotNull(tag), .. args]);
}

public abstract class Node<TNode>
    : INode
    where TNode : Node<TNode> {
    protected Node(string? tag, IServiceProvider services) {
        Services = services;
        var idProvider = services.GetRequiredService<INodeSequencer>();
        Id = idProvider.Current;
        Tag = tag;
    }

    public string Label { get; protected init; } = typeof(TNode).Name;

    protected IServiceProvider Services { get; }

    public uint Id { get; }
    public Token? Token { get; init; }
    public string? Tag { get; init; }

    public INode? Next { get; set; }

    public Result Validate(ISet<INode>? visited = null) {
        visited ??= new HashSet<INode>();
        return !visited.Add(this)
            ? Success()
            : IsValid(visited);
    }

    public abstract void ConnectTo(INode? next);

    protected virtual Result IsValid(ISet<INode> visited) => Success();

    public async Task<INode?> Run(IMap context, CancellationToken ct = default) {
        await UpdateState(context, ct);
        return await SelectPath(context, ct);
    }

    protected abstract Task UpdateState(IMap context, CancellationToken ct = default);
    protected abstract Task<INode?> SelectPath(IMap context, CancellationToken ct = default);

    public override int GetHashCode() => HashCode.Combine(Id, Tag ?? string.Empty);
}
