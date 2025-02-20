namespace DotNetToolbox.Graph.Builders;

public sealed class WorkflowBuilder(IServiceProvider services)
    : IWorkflowBuilder,
      IExitBuilder,
      IIfBuilder,
      IElseBuilder,
      IOtherwiseBuilder,
      IActionBuilder {
    private readonly List<INode> _nodes = [];

    private INode? _first;
    private INode? _current;

    public INode Build() {
        ConnectJumps();
        return GetStart();
    }

    private INode GetStart()
        => _first ?? new ExitNode(services);

    public IWorkflowBuilder AddNode(INode node) {
        Connect(node);
        return this;
    }

    public IWorkflowBuilder Do<TAction>(params object[] args)
        where TAction : ActionNode<TAction> {
        AddNode(Node.Create<TAction>(string.Empty, services, args));
        return this;
    }
    public IWorkflowBuilder Do<TAction>(string tag, params object[] args)
        where TAction : ActionNode<TAction> {
        AddNode(Node.Create<TAction>(tag, services, args));
        return this;
    }

    public IActionBuilder Do(string name) {
        AddNode(new ActionNode(name, services));
        return this;
    }
    public IActionBuilder Do(string tag, string name) {
        AddNode(new ActionNode(tag, name, services));
        return this;
    }
    public IActionBuilder Do(Action<IMap> action) {
        AddNode(new ActionNode(action, services));
        return this;
    }
    public IActionBuilder Do(Func<IMap, CancellationToken, Task> action) {
        AddNode(new ActionNode(action, services));
        return this;
    }
    public IActionBuilder Do(string tag, Action<IMap> action) {
        AddNode(new ActionNode(tag, action, services));
        return this;
    }
    public IActionBuilder Do(string tag, Func<IMap, CancellationToken, Task> action) {
        AddNode(new ActionNode(tag, action, services));
        return this;
    }

    public IIfBuilder If(string name) {
        AddNode(new IfNode(name, services));
        return this;
    }
    public IIfBuilder If(string tag, string name) {
        AddNode(new IfNode(tag, name, services));
        return this;
    }
    public IIfBuilder If(Func<IMap, bool> predicate) {
        AddNode(new IfNode(predicate, services));
        return this;
    }
    public IIfBuilder If(Func<IMap, CancellationToken, Task<bool>> predicate) {
        AddNode(new IfNode(predicate, services));
        return this;
    }
    public IIfBuilder If(string tag, Func<IMap, bool> predicate) {
        AddNode(new IfNode(tag, predicate, services));
        return this;
    }
    public IIfBuilder If(string tag, Func<IMap, CancellationToken, Task<bool>> predicate) {
        AddNode(new IfNode(tag, predicate, services));
        return this;
    }
    public IElseBuilder Then(Action<IWorkflowBuilder> setThen) {
        var builder = new WorkflowBuilder(services);
        setThen(builder);
        ((IIfNode)_current!).Then = builder.GetStart();
        _nodes.AddRange(builder._nodes);
        return this;
    }
    public IWorkflowBuilder Else(Action<IWorkflowBuilder> setElse) {
        var builder = new WorkflowBuilder(services);
        setElse(builder);
        ((IIfNode)_current!).Else = builder.GetStart();
        _nodes.AddRange(builder._nodes);
        return this;
    }

    public ICaseBuilder Case(string selector) {
        AddNode(new CaseNode(selector, services));
        return this;
    }
    public ICaseBuilder Case(string tag, string selector) {
        AddNode(new CaseNode(tag, selector, services));
        return this;
    }
    public ICaseBuilder Case(Func<IMap, string> select) {
        AddNode(new CaseNode(select, services));
        return this;
    }
    public ICaseBuilder Case(Func<IMap, CancellationToken, Task<string>> select) {
        AddNode(new CaseNode(select, services));
        return this;
    }
    public ICaseBuilder Case(string? tag, Func<IMap, string> select) {
        AddNode(new CaseNode(tag, select, services));
        return this;
    }
    public ICaseBuilder Case(string tag, Func<IMap, CancellationToken, Task<string>> select) {
        AddNode(new CaseNode(tag, select, services));
        return this;
    }
    public IOtherwiseBuilder Is(string key, Action<IWorkflowBuilder> setCase) {
        var builder = new WorkflowBuilder(services);
        setCase(builder);
        ((ICaseNode)_current!).Choices[IsNotNullOrWhiteSpace(key)] = builder.Build();
        _nodes.AddRange(builder._nodes);
        return this;
    }
    public IWorkflowBuilder Otherwise(Action<IWorkflowBuilder> setOtherwise) {
        var builder = new WorkflowBuilder(services);
        setOtherwise(builder);
        ((ICaseNode)_current!).Choices[string.Empty] = builder.Build();
        _nodes.AddRange(builder._nodes);
        return this;
    }

    public IWorkflowBuilder GoTo(string nodeTag) {
        AddNode(new JumpNode(nodeTag, services));
        return this;
    }

    public IExitBuilder Exit(int exitCode = 0) {
        AddNode(new ExitNode(exitCode, services));
        return this;
    }
    public IExitBuilder Exit(string tag, int exitCode = 0) {
        AddNode(new ExitNode(tag, exitCode, services));
        return this;
    }

    public IWorkflowBuilder WithRetry(IRetryPolicy retry) {
        ((IActionNode)_current!).Retry = retry;
        return this;
    }

    private void Connect(INode node) {
        _nodes.Add(IsNotNull(node));
        _first ??= node;
        _current?.ConnectTo(node);
        _current = node;
    }

    private void ConnectJumps() {
        foreach (var jumpNode in _nodes.OfType<IJumpNode>()) {
            var targetNodeTag = _nodes.Find(n => n.Tag == jumpNode.TargetTag)
                             ?? throw new ResultException($"Jump target '{jumpNode.TargetTag}' not found.", jumpNode.Token?.ToSource() ?? string.Empty);
            jumpNode.ConnectTo(targetNodeTag);
        }
    }
}
