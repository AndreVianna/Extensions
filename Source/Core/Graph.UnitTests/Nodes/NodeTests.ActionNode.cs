namespace DotNetToolbox.Graph.Nodes;

public partial class NodeTests {
    public class ActionNodeTests : NodeTests {
        [Fact]
        public void CreateAction_WithoutTag_ReturnsActionNodeWithDefaultLabel() {
            var node = CreateFactory().CreateAction(_ => { });

            node.Should().NotBeNull();
            node.Should().BeOfType<ActionNode>();
            node.Id.Should().Be(1);
            node.Tag.Should().BeNull();
            node.Label.Should().Be("action");
        }

        [Fact]
        public void CreateAction_WithCustomTag_ReturnsActionNodeWithCustomLabel() {
            const string customTag = "Action1";
            var node = CreateFactory().CreateAction(customTag, _ => { });

            node.Should().NotBeNull();
            node.Should().BeOfType<ActionNode>();
            node.Tag.Should().Be(customTag);
            node.Label.Should().Be("action");
        }

        [Fact]
        public void CreateAction_WithCustomPolicy_AppliesPolicyToNode() {
            var policy = new TestRetryPolicy();
            var node = CreateFactory(policy).CreateAction(_ => { });

            node.Should().NotBeNull();
            node.Should().BeOfType<ActionNode>();
        }

        [Fact]
        public async Task Run_WithSuccessfulRetry_ExecutesPolicyAndAction() {
            var policy = new TestRetryPolicy(failedTries: 2);
            var actionExecuted = false;
            var node = CreateFactory(policy).CreateAction(_ => actionExecuted = true);
            var context = new Map();

            await node.Run(context);

            policy.TryCount.Should().Be(3);
            actionExecuted.Should().BeTrue();
        }

        [Fact]
        public async Task Run_WithTooManyRetries_ExecutesPolicyAndAction() {
            var policy = new TestRetryPolicy(failedTries: RetryPolicy.DefaultMaximumRetries + 1);
            var actionExecuted = false;
            var node = CreateFactory(policy).CreateAction(_ => actionExecuted = true);
            var context = new Map();

            var action = () => node.Run(context);

            await action.Should().ThrowAsync<PolicyException>();
            policy.TryCount.Should().Be(RetryPolicy.DefaultMaximumRetries);
            actionExecuted.Should().BeTrue();
        }

        [Fact]
        public async Task Run_WithCustomRetries_ExecutesPolicyAndAction() {
            var policy = new TestRetryPolicy(maxRetries: 10, failedTries: 11);
            var actionExecuted = false;
            var node = CreateFactory(policy).CreateAction(_ => actionExecuted = true);
            var context = new Map();

            var action = () => node.Run(context);

            await action.Should().ThrowAsync<PolicyException>();
            policy.TryCount.Should().Be(10);
            actionExecuted.Should().BeTrue();
        }

        [Fact]
        public async Task Run_RetryOnException_ExecutesPolicyAndAction() {
            var policy = new TestRetryPolicy(failedTries: RetryPolicy.DefaultMaximumRetries + 1);
            var node = CreateFactory(policy).CreateAction(_ => throw new());
            var context = new Map();

            var action = () => node.Run(context);

            await action.Should().ThrowAsync<PolicyException>();
            policy.TryCount.Should().Be(RetryPolicy.DefaultMaximumRetries);
        }

        [Fact]
        public async Task Run_WithRetryAsMax_IgnoresPolicyAndTryAsManyTimesAsNeeded() {
            var policy = new TestRetryPolicy(maxRetries: byte.MaxValue);
            var node = CreateFactory(policy).CreateAction(_ => {
                if (policy.TryCount < 10) throw new();
            });
            var context = new Map();

            var action = () => node.Run(context);

            await action.Should().NotThrowAsync();
            policy.TryCount.Should().Be(10);
        }

        [Fact]
        public async Task Run_RunMethod_UpdatesContextAndReturnsNextNode() {
            var context = new Map();
            var node = CreateFactory().CreateAction("2", ctx => ctx["key"] = "value");
            node.Next = CreateFactory().CreateAction(_ => { });

            var result = await node.Run(context);

            result.Should().BeSameAs(node.Next);
            context["key"].Should().Be("value");
        }

        private sealed class TestRetryPolicy(byte maxRetries = RetryPolicy.DefaultMaximumRetries, uint failedTries = 0)
            : RetryPolicy(maxRetries) {
            protected override async Task<bool> TryExecute(Func<IMap, CancellationToken, Task> action, IMap ctx, CancellationToken ct) {
                TryCount++;
                await action(ctx, ct);
                return TryCount > failedTries;
            }
            public uint TryCount { get; private set; }
        }
    }
}
