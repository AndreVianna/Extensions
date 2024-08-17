namespace DotNetToolbox.Graph.Nodes;

public class ConditionalNodeTests {
    private readonly ServiceProvider _provider;
    private readonly NodeFactory _factory;

    public ConditionalNodeTests() {
        var services = new ServiceCollection();
        services.AddTransient<IPolicy, RetryPolicy>();
        services.AddTransient<INodeFactory>(p => new NodeFactory(p));
        _provider = services.BuildServiceProvider();
        _factory = new NodeFactory(_provider);
    }

    [Fact]
    public void CreateFork_WithoutLabel_ReturnsConditionalNodeWithDefaultLabel() {
        var node = _factory.CreateFork(1,
                                       _ => true,
                                       b => b.IsTrue(t => t.Do(_ => { })));

        node.Should().NotBeNull();
        node.Should().BeOfType<IfNode>();
        node.Label.Should().Be("if");
    }

    [Fact]
    public void CreateFork_WithCustomLabel_ReturnsConditionalNodeWithCustomLabel() {
        const string customLabel = "Custom Fork";
        const string customTag = "Action1";
        var node = _factory.CreateFork(1,
                                       _ => true,
                                       b => b.IsTrue(t => t.Do(_ => { })),
                                       customTag,
                                       customLabel);

        node.Should().NotBeNull();
        node.Should().BeOfType<IfNode>();
        node.Label.Should().Be(customLabel);
    }

    [Fact]
    public void CreateFork_WithTrueBranchOnly_SetsOnlyTrueBranch() {
        var node = _factory.CreateFork(1,
                                       _ => true,
                                       b => b.IsTrue(t => t.Do(_ => { })));

        var ifNode = node.Should().BeOfType<IfNode>().Subject;
        ifNode.IsTrue.Should().NotBeNull();
        ifNode.IsFalse.Should().BeNull();
    }

    [Fact]
    public void CreateFork_WithBothBranches_SetsBothBranches() {
        var node = _factory.CreateFork(1, _ => true,
                                       b => b.IsTrue(t => t.Do(_ => { }))
                                             .IsFalse(f => f.Do(_ => { })));

        var ifNode = node.Should().BeOfType<IfNode>().Subject;
        ifNode.IsTrue.Should().NotBeNull();
        ifNode.IsFalse.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateFork_RunMethodWithTrueCondition_ExecutesTrueBranch() {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        using var context = new Context(provider);
        var node = _factory.CreateFork(1, _ => true,
                                       b => b.IsTrue(t => t.Do(ctx => ctx["branch"] = "true"))
                                             .IsFalse(f => f.Do(ctx => ctx["branch"] = "false")));

        await node.Run(context);

        context["branch"].Should().Be("true");
    }

    private sealed class CustomContext(IServiceProvider provider)
        : Context(provider);

    [Fact]
    public async Task CreateFork_RunMethodWithFalseCondition_ExecutesFalseBranch() {
        var services = new ServiceCollection();
        var provider = services.BuildServiceProvider();
        var context = new CustomContext(provider) {
            ["Disposable"] = new CustomContext(provider),
        };
        var node = _factory.CreateFork(1, _ => false,
                                       b => b.IsTrue(t => t.Do(ctx => ctx["branch"] = "true"))
                                             .IsFalse(f => f.Do(ctx => ctx["branch"] = "false")));

        await node.Run(context);

        context["branch"].Should().Be("false");
        context.Dispose();
        context.Dispose();
    }

    [Fact]
    public void CreateFork_ValidateMethod_ValidatesBothBranches() {
        var node = _factory.CreateFork(1, _ => true,
                                       b => b.IsTrue(t => t.Do(_ => { }))
                                             .IsFalse(f => f.Do(_ => { })));

        var result = node.Validate();

        result.IsSuccess.Should().BeTrue();
    }
}
