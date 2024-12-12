namespace DotNetToolbox.Graph.Nodes;

public sealed class JumpNode : JumpNode<JumpNode> {
    public JumpNode(string targetTag, IServiceProvider services)
        : base(services) {
        Label = "goto";
        TargetTag = IsNotNull(targetTag);
    }

    public override string TargetTag { get; }
}

public abstract class JumpNode<TNode>(IServiceProvider services)
    : Node<TNode>(null, services),
      IJumpNode
    where TNode : JumpNode<TNode> {
    protected override Task<INode?> SelectPath(IMap context, CancellationToken ct = default)
        => Task.FromResult(Next);

    protected override Task UpdateState(IMap context, CancellationToken ct = default)
        => Task.CompletedTask;

    public override void ConnectTo(INode? next)
        => Next = next;

    public abstract string TargetTag { get; }
}
