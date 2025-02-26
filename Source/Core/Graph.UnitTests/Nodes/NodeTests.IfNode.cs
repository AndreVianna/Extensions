namespace DotNetToolbox.Graph.Nodes;

public partial class NodeTests {
    public class IfNodeTests : NodeTests {
        [Fact]
        public void CreateIf_WithoutTag_ReturnsConditionalNodeWithDefaultLabel() {
            var node = CreateFactory().CreateIf(_ => true);

            node.Should().NotBeNull();
            node.Should().BeOfType<IfNode>();
            node.Id.Should().Be(1);
            node.Label.Should().Be("if");

            node.Then.Should().BeNull();
            node.Else.Should().BeNull();
        }

        [Fact]
        public void CreateIf_WithCustomTag_ReturnsConditionalNodeWithCustomLabel() {
            const string customTag = "Action1";
            var node = CreateFactory().CreateIf(customTag, _ => true);

            node.Should().NotBeNull();
            node.Should().BeOfType<IfNode>();
            node.Tag.Should().Be(customTag);
            node.Label.Should().Be("if");

            node.Then.Should().BeNull();
            node.Else.Should().BeNull();
        }

        [Fact]
        public void CreateIf_WithTrueBranchOnly_SetsOnlyTrueBranch() {
            var node = CreateFactory().CreateIf(_ => true,
                                                CreateFactory().CreateAction(_ => { }));

            var ifNode = node.Should().BeOfType<IfNode>().Subject;
            ifNode.Then.Should().NotBeNull();
            ifNode.Else.Should().BeNull();
        }

        [Fact]
        public void CreateIf_WithBothBranches_SetsBothBranches() {
            var node = CreateFactory().CreateIf(_ => true,
                                                CreateFactory().CreateAction(_ => { }),
                                                CreateFactory().CreateAction(_ => { }));

            var ifNode = node.Should().BeOfType<IfNode>().Subject;
            ifNode.Then.Should().NotBeNull();
            ifNode.Else.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateIf_RunMethodWithTrueCondition_ExecutesTrueBranch() {
            using var context = new Map();
            var node = CreateFactory().CreateIf(_ => true,
                                                CreateFactory().CreateAction(ctx => ctx["branch"] = "true"),
                                                CreateFactory().CreateAction(ctx => ctx["branch"] = "false"));

            await node.Run(context);

            context["branch"].Should().Be("true");
        }

        [Fact]
        public async Task CreateIf_RunMethodWithFalseCondition_ExecutesFalseBranch() {
            var context = new Map() {
                ["Disposable"] = new Map(),
            };
            var node = CreateFactory().CreateIf(_ => false,
                                                CreateFactory().CreateAction(ctx => ctx["branch"] = "true"),
                                                CreateFactory().CreateAction(ctx => ctx["branch"] = "false"));

            await node.Run(context);

            context["branch"].Should().Be("false");
            context.Dispose();
            context.Dispose();
        }

        [Fact]
        public void CreateIf_ValidateMethod_ValidatesBothBranches() {
            var node = CreateFactory().CreateIf(_ => true,
                                                CreateFactory().CreateAction(ctx => ctx["branch"] = "true"),
                                                CreateFactory().CreateAction(ctx => ctx["branch"] = "false"));

            var result = node.Validate();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}
