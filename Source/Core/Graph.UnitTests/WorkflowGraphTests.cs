namespace DotNetToolbox.Graph;

public sealed class WorkflowGraphTests {
    private readonly IWorkflowBuilder _builder;

    public WorkflowGraphTests() {
        var services = new ServiceCollection();
        services.AddSingleton<INodeFactory, NodeFactory>();
        services.AddScoped<INodeSequencer, NodeSequencer>();
        services.AddScoped<IRetryPolicy, RetryPolicy>();
        var provider = services.BuildServiceProvider();
        _builder = new WorkflowBuilder(provider);
    }

    [Fact]
    public void BuildGraph_SingleAction_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["action"]

                                      """;
        var wf = _builder.Do(_ => { }).Build();

        var graph = WorkflowGraph.Draw(wf);

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_WithCircularReference_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["wf"]
                                      1 --> 2
                                      2["action"]
                                      2 --> 3
                                      3["goto"]
                                      3 --> 1

                                      """;
        var wf = _builder.Do("wf", _ => { })
                            .Do(_ => { })
                            .GoTo("wf")
                            .Build();

        var graph = WorkflowGraph.Draw(wf);

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_MultipleConnectedActions_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["action"]
                                      1 --> 2
                                      2["action"]
                                      2 --> 3
                                      3["action"]
                                      3 --> 4
                                      4["action"]

                                      """;

        var wf = _builder.Do(_ => { })
                         .Do(_ => { })
                         .Do(_ => { })
                         .Do(_ => { })
                         .Build();

        var graph = WorkflowGraph.Draw(wf);

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_IfNode_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["if"]
                                      1 --> |True| 2
                                      2["action"]
                                      1 --> |False| 3
                                      3["action"]

                                      """;
        var wf = _builder.If(_ => true)
                            .Then(t => t.Do(_ => { }))
                            .Else(f => f.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf);

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_IfNode_WithGroupEdges_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["if"]
                                      1 --> |True| 2
                                      1 --> |False| 3
                                      2["action"]
                                      3["action"]

                                      """;
        var wf = _builder.If(_ => true)
                            .Then(t => t.Do(_ => { }))
                            .Else(f => f.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf, c => c.Format(GraphFormat.GroupedEdges));

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_IfNode_Indented_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["if"]
                                      1 --> |True| 2
                                          2["action"]
                                      1 --> |False| 3
                                          3["action"]

                                      """;
        var wf = _builder.If(_ => true)
                            .Then(t => t.Do(_ => { }))
                            .Else(f => f.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf, c => c.Format(GraphFormat.Indented));

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_SelectNode_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["case"]
                                      1 --> |key1| 2
                                      2["action"]
                                      1 --> |key2| 3
                                      3["action"]
                                      1 --> |key3| 4
                                      4["action"]

                                      """;
        var wf = _builder.Case(_ => "key1")
                            .Is("key1", b => b.Do(_ => { }))
                            .Is("key2", b => b.Do(_ => { }))
                            .Is("key3", b => b.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf);

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_SelectNode_WithGroupEdges_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["case"]
                                      1 --> |key1| 2
                                      1 --> |key2| 3
                                      1 --> |key3| 4
                                      2["action"]
                                      3["action"]
                                      4["action"]

                                      """;
        var wf = _builder.Case(_ => "key1")
                            .Is("key1", b => b.Do(_ => { }))
                            .Is("key2", b => b.Do(_ => { }))
                            .Is("key3", b => b.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf, c => c.Format(GraphFormat.GroupedEdges));

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_SelectNode_Indented_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["case"]
                                      1 --> |key1| 2
                                          2["action"]
                                      1 --> |key2| 3
                                          3["action"]
                                      1 --> |key3| 4
                                          4["action"]

                                      """;
        var wf = _builder.Case(_ => "key1")
                            .Is("key1", b => b.Do(_ => { }))
                            .Is("key2", b => b.Do(_ => { }))
                            .Is("key3", b => b.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf, c => c.Format(GraphFormat.Indented));

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_ComplexWorkflow_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["action"]
                                      1 --> 2
                                      2["if"]
                                      2 --> |True| 3
                                      3["action"]
                                      3 --> 4
                                      4["case"]
                                      4 --> |key1| 5
                                      5["action"]
                                      4 --> |key2| 6
                                      6["action"]
                                      2 --> |False| 7
                                      7["action"]

                                      """;

        var wf = _builder.Do(_ => { })
                            .If(_ => true)
                            .Then(t => t.Do(_ => { })
                                        .Case(_ => "key1")
                                        .Is("key1", b => b.Do(_ => { }))
                                        .Is("key2", b => b.Do(_ => { })))
                            .Else(f => f.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf);

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_ComplexWorkflow_GroupedEdges_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["action"]
                                      1 --> 2
                                      2["if"]
                                      2 --> |True| 3
                                      2 --> |False| 7
                                      3["action"]
                                      3 --> 4
                                      4["case"]
                                      4 --> |key1| 5
                                      4 --> |key2| 6
                                      5["action"]
                                      6["action"]
                                      7["action"]

                                      """;

        var wf = _builder.Do(_ => { })
                            .If(_ => true)
                            .Then(t => t.Do(_ => { })
                                        .Case(_ => "key1")
                                        .Is("key1", b => b.Do(_ => { }))
                                        .Is("key2", b => b.Do(_ => { })))
                            .Else(f => f.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf, c => c.Format(GraphFormat.GroupedEdges));

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_ComplexWorkflow_Indexted_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["action"]
                                      1 --> 2
                                      2["if"]
                                      2 --> |True| 3
                                          3["action"]
                                          3 --> 4
                                          4["case"]
                                          4 --> |key1| 5
                                              5["action"]
                                          4 --> |key2| 6
                                              6["action"]
                                      2 --> |False| 7
                                          7["action"]

                                      """;

        var wf = _builder.Do(_ => { })
                            .If(_ => true)
                            .Then(t => t.Do(_ => { })
                                        .Case(_ => "key1")
                                        .Is("key1", b => b.Do(_ => { }))
                                        .Is("key2", b => b.Do(_ => { })))
                            .Else(f => f.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf, c => c.Format(GraphFormat.Indented));

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_WithLabels_IncludesLabelsInMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["Start"]
                                      1 --> 2
                                      2["Decision"]
                                      2 --> |True| 3
                                      3["Success"]
                                      2 --> |False| 4
                                      4["Fail"]

                                      """;
        var wf = _builder.Do("Start", _ => { })
                            .If("Decision", _ => true)
                            .Then(t => t.Do("Success", _ => { }))
                            .Else(f => f.Do("Fail", _ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf);

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_WithLoop_HandlesLoopCorrectly() {
        const string expectedResult = """
                                      flowchart TD
                                      1["action"]
                                      1 --> 2
                                      2["Loop"]
                                      2 --> |True| 3
                                      3["action"]
                                      3 --> 4
                                      4["goto"]
                                      4 --> 2
                                      2 --> |False| 5
                                      5["exit"]

                                      """;
        var wf = _builder.Do(_ => { })
                            .If("Loop", _ => true)
                            .Then(t => t.Do(_ => { })
                                        .GoTo("Loop"))
                            .Exit()
                            .Build();
        var graph = WorkflowGraph.Draw(wf);

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_WithGenericAction_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart TD
                                      1["CustomAction"]

                                      """;
        var wf = _builder.Do<CustomAction>().Build();

        var graph = WorkflowGraph.Draw(wf);

        graph.Should().Be(expectedResult);
    }

    [Fact]
    public void BuildGraph_WithNestedConditions_ReturnsCorrectMermaidChart() {
        const string expectedResult = """
                                      flowchart LR
                                      1["if"]
                                      1 --> |True| 2
                                      2["if"]
                                      2 --> |True| 3
                                      3["action"]
                                      2 --> |False| 4
                                      4["action"]
                                      1 --> |False| 5
                                      5["action"]

                                      """;
        var wf = _builder.If(_ => true)
                            .Then(t1 => t1.If(_ => false)
                                          .Then(t2 => t2.Do(_ => { }))
                                          .Else(f2 => f2.Do(_ => { })))
                            .Else(f1 => f1.Do(_ => { }))
                            .Build();

        var graph = WorkflowGraph.Draw(wf, c => c.Direction(GraphDirection.Horizontal));

        graph.Should().Be(expectedResult);
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Test class
    private sealed class CustomAction(string? tag, IServiceProvider services)
                : ActionNode<CustomAction>(tag, services) {
        protected override Task Execute(IMap context, CancellationToken ct = default)
            => Task.CompletedTask;
    }
}
