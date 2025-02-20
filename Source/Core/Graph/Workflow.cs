﻿namespace DotNetToolbox.Graph;

public class Workflow
    : Workflow<Map>,
      IWorkflow {
    public Workflow(string id,
                    INode start,
                    Map context,
                    IDateTimeProvider? dateTime = null,
                    ILoggerFactory? loggerFactory = null)
        : base(id,
               start,
               context,
               dateTime,
               loggerFactory) {
    }

    public Workflow(INode start,
                    Map context,
                    IDateTimeProvider? dateTime = null,
                    ILoggerFactory? loggerFactory = null)
        : base(start,
               context,
               dateTime,
               loggerFactory) {
    }
}

public class Workflow<TContext>(string id,
                      INode start,
                      TContext context,
                      IDateTimeProvider? dateTime = null,
                      ILoggerFactory? loggerFactory = null)
    : IWorkflow<TContext>
    where TContext : IMap {
    private uint _runCount;

    public Workflow(INode start,
                    TContext context,
                    IDateTimeProvider? dateTime = null,
                    ILoggerFactory? loggerFactory = null)
        : this(StringGuidProvider.Default.CreateSortable(),
               start,
               context,
               dateTime,
               loggerFactory) {
    }

    public string Id { get; } = id;
    public INode StartNode { get; } = IsNotNull(start);
    public TContext Context { get; } = IsNotNull(context);

    public IValidationResult Validate() => ValidateNode(StartNode);

    private static IValidationResult ValidateNode(INode? node, ISet<INode>? visited = null) {
        if (node is null) return Success();
        visited ??= new HashSet<INode>();
        var result = !visited.Add(node)
                         ? Success()
                         : node.Validate(visited);
        switch (node) {
            case IActionNode n:
                result = result.Add(ValidateNode(n.Next, visited));
                break;
            case IIfNode n:
                result = result.Add(ValidateNode(n.Then, visited));
                result = result.Add(ValidateNode(n.Else, visited));
                break;
            case ICaseNode n:
                foreach ((_, var branch) in n.Choices)
                    result = result.Add(ValidateNode(branch, visited));
                break;
        }
        return result;
    }

    public Task Run(CancellationToken ct = default) {
        var runner = new Runner<TContext>(++_runCount, this, dateTime, loggerFactory);
        return runner.Run(ct);
    }
}
