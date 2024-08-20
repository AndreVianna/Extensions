﻿namespace DotNetToolbox.Graph.Nodes;

public static class Node {
    [return: NotNull]
    public static TNode Create<TNode>(IServiceProvider services, params object?[] args)
        where TNode : Node<TNode>
        => InstanceFactory.Create<TNode>(services, args);
}

public abstract class Node<TNode>
    : INode
    where TNode : Node<TNode> {

    protected Node(string? id, INodeSequence? sequence) {
        Number = (sequence ?? NodeSequence.Singleton).Next;
        Id = string.IsNullOrWhiteSpace(id) ? $"{Number}" : id;
        Label = string.IsNullOrWhiteSpace(id) ? DefaultLabel : Id;
    }

    protected virtual string DefaultLabel { get; } = typeof(TNode).Name;

    public string Id { get; }
    public uint Number { get; }
    public string Label { get; set; }
    public Token? Token { get; set; }
    public INode? Next { get; set; }

    public Result Validate(ISet<INode>? visited = null) {
        visited ??= new HashSet<INode>();
        return !visited.Add(this)
            ? Success()
            : IsValid(visited);
    }

    public abstract void ConnectTo(INode? next);

    protected virtual Result IsValid(ISet<INode> visited) => Success();

    public async Task<INode?> Run(Context context, CancellationToken ct = default) {
        await UpdateState(context, ct);
        return await SelectPath(context, ct);
    }

    protected abstract Task UpdateState(Context context, CancellationToken ct = default);
    protected abstract Task<INode?> SelectPath(Context context, CancellationToken ct = default);

    public override int GetHashCode() => Id.GetHashCode();

    internal void MapTo(Token token, Dictionary<INode, Token> nodeMap)
        => nodeMap[this] = token;
}
