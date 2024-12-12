namespace DotNetToolbox.Graph.Nodes;

public interface INode {
    uint Id { get; }
    string Label { get; }
    Token? Token { get; }
    string? Tag { get; }

    INode? Next { get; set; }

    void ConnectTo(INode? next);
    Result Validate(ISet<INode>? visited = null);
    Task<INode?> Run(IMap context, CancellationToken ct = default);
}
