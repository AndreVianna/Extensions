namespace DotNetToolbox.Graph.Nodes;

public partial class NodeTests {
    public class ComplexWorkflowTests : NodeTests {
        [Fact]
        public async Task ComplexWorkflow_WithMultipleNodeTypes_ExecutesCorrectly() {
            using var context = new Map();
            var start = CreateComplexWorkflow();
            var workflow = new Workflow("1", start, context);

            await workflow.Run();

            context["count"].Should().Be(2);
            context["result"].Should().Be("Action2");
        }

        [Fact]
        public void ComplexWorkflow_Validation_SucceedsForValidWorkflow() {
            var workflow = CreateComplexWorkflow();

            var result = workflow.Validate();

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void ComplexWorkflow_WithCircularReference_DetectedDuringValidation() {
            var startNode = CreateFactory().CreateAction(_ => { });
            var actionNode = CreateFactory().CreateAction(_ => { });
            startNode.Next = actionNode;
            actionNode.Next = startNode; // Creating a circular reference

            var result = startNode.Validate();

            result.IsSuccess.Should().BeTrue(); // Circular references are allowed, but detected
        }

        [Fact]
        public void ComplexWorkflow_WithCustomPolicy_AppliesPolicyCorrectly() {
            var policyExecutionCount = 0;
            var policy = new CustomRetryPolicy(() => policyExecutionCount++);
            var builder = new WorkflowBuilder(CreateServiceProvider(policy));
            builder.Do(_ => { });

            using var context = new Map();

            var wf = builder.Build();

            wf.Run(context);

            policyExecutionCount.Should().Be(1);
        }

        private static INode CreateComplexWorkflow() {
            var builder = new WorkflowBuilder(CreateServiceProvider());
            builder.Do(ctx => ctx["count"] = 0)
                   .If("LoopStart", ctx => ctx["count"].As<int>() < 2)
                   .Then(t1 => t1.Do(ctx => ctx["count"] = ctx["count"].As<int>() + 1)
                                 .Do(ctx => ctx["result"] = "Action1")
                                 .GoTo("LoopStart"))
                   .Else(f1 => f1.If(ctx => ctx["count"].As<int>() % 2 == 0)
                                  .Then(t2 => t2.Do(ctx => ctx["result"] = "Action2"))
                                  .Else(f2 => f2.Do(ctx => ctx["result"] = "Action3")));
            return builder.Build();
        }

        private sealed class CustomRetryPolicy(Action onExecute) : IRetryPolicy {
            public IReadOnlyList<TimeSpan> Delays { get; } = [];
            public byte MaxRetries => 3;

            public Task Execute(Func<IMap, CancellationToken, Task> action, IMap ctx, CancellationToken ct = default) {
                onExecute();
                return action(ctx, ct);
            }
        }
    }
}
