namespace DotNetToolbox.Graph.Nodes;

public sealed class CaseNode : CaseNode<CaseNode> {
    private CaseNode(string? tag, string? name, Func<IMap, CancellationToken, Task<string>>? select, IServiceProvider services)
        : base(tag, services) {
        Name = name ?? Name;
        Label = name ?? "case";
        _select = select!;
    }
    public CaseNode(string? name, IServiceProvider services)
        : this(null, name, null, services) {
    }
    public CaseNode(string? tag, string? name, IServiceProvider services)
        : this(tag, name, null, services) {
    }
    public CaseNode(Func<IMap, CancellationToken, Task<string>> select, IServiceProvider services)
        : this(null, null, select, services) {
    }
    public CaseNode(string? tag, Func<IMap, CancellationToken, Task<string>> select, IServiceProvider services)
        : this(tag, null, select, services) {
    }
    public CaseNode(Func<IMap, string> select, IServiceProvider services)
        : this(null, null, (ctx, ct) => Task.Run(() => select(ctx), ct), services) {
    }
    public CaseNode(string? tag, Func<IMap, string> select, IServiceProvider services)
        : this(tag, null, (ctx, ct) => Task.Run(() => select(ctx), ct), services) {
    }

    private readonly Func<IMap, CancellationToken, Task<string>> _select;

    protected override Task<string> Select(IMap context, CancellationToken ct = default) => _select(context, ct);
}

public abstract class CaseNode<TNode>(string? tag, IServiceProvider services)
    : Node<TNode>(tag, services),
      ICaseNode
    where TNode : CaseNode<TNode> {
    public string Name { get; set; } = typeof(TNode).Name;
    public Dictionary<string, INode?> Choices { get; } = [];

    protected override Result IsValid(ISet<INode> visited) {
        var result = base.IsValid(visited);
        if (Choices.Count == 0)
            result += new OperationError("The case node has no choices.", Token?.ToSource());
        var choices = Choices.Values
                             .Where(c => c is not null)
                             .Cast<INode>()
                             .Distinct();
        return choices.Aggregate(result, ValidateChoice);

        Result ValidateChoice(Result current, INode choice)
            => current + choice.Validate(visited);
    }

    protected override async Task<INode?> SelectPath(IMap context, CancellationToken ct = default) {
        ct.ThrowIfCancellationRequested();
        var key = await Select(context, ct);
        var choice = Choices.GetValueOrDefault(key)
            ?? Choices.GetValueOrDefault(string.Empty)
            ?? throw new InvalidOperationException($"The path '{key}' was not found.");
        return await choice.Run(context, ct);
    }

    protected abstract Task<string> Select(IMap context, CancellationToken ct = default);

    protected sealed override Task UpdateState(IMap context, CancellationToken ct = default)
        => Task.CompletedTask;

    public sealed override void ConnectTo(INode? next) {
        foreach (var choice in Choices)
            choice.Value?.ConnectTo(next);
    }
}
