namespace DotNetToolbox.Graph.Nodes;

public sealed class ExitNode : ExitNode<ExitNode> {
    public ExitNode(string? tag, int exitCode, IServiceProvider services)
        : base(tag, services) {
        Label = "exit";
        ExitCode = exitCode;
    }
    public ExitNode(int exitCode, IServiceProvider services)
        : this(null, exitCode, services) {
    }
    public ExitNode(IServiceProvider services)
        : this(null, 0, services) {
    }
    public ExitNode(string? tag, IServiceProvider services)
        : this(tag, 0, services) {
    }

    public override int ExitCode { get; }
}

public abstract class ExitNode<TNode>(string? tag, IServiceProvider services)
    : Node<TNode>(tag, services),
      IExitNode
    where TNode : ExitNode<TNode> {
    protected override Task<INode?> SelectPath(IMap context, CancellationToken ct = default)
        => Task.FromResult<INode?>(null);

    protected override Task UpdateState(IMap context, CancellationToken ct = default)
        => Task.CompletedTask;

    public override void ConnectTo(INode? next)
        => throw new OperationFailureException(new Error("An exit node cannot be connected to another node.", Token?.ToSource() ?? Tag ?? $"{Id}"));

    public abstract int ExitCode { get; }
}
