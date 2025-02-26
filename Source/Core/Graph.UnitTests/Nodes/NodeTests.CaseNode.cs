namespace DotNetToolbox.Graph.Nodes;

public partial class NodeTests {
    public class CaseNodeTests : NodeTests {
        [Fact]
        public void CreateCase_WithoutTag_ReturnsBranchingNodeWithDefaultLabel() {
            var node = CreateFactory().CreateCase(_ => "default");

            node.Should().NotBeNull();
            node.Should().BeOfType<CaseNode>();
            node.Id.Should().Be(1);
            node.Tag.Should().BeNull();
            node.Label.Should().Be("case");

            node.Choices.Should().BeEmpty();
        }

        [Fact]
        public void CreateCase_WithCustomTag_ReturnsBranchingNodeWithCustomLabel() {
            const string customId = "Action1";
            var node = CreateFactory().CreateCase(customId, _ => "default");

            node.Should().NotBeNull();
            node.Should().BeOfType<CaseNode>();
            node.Tag.Should().Be(customId);
            node.Label.Should().Be("case");

            node.Choices.Should().BeEmpty();
        }

        [Fact]
        public void CreateCase_WithMultipleBranches_SetsAllBranches() {
            var node = CreateFactory().CreateCase("1",
                                             _ => "key",
                                             new() {
                                                 ["key1"] = null,
                                                 ["key2"] = null,
                                                 ["key3"] = null,
                                             },
                                             CreateFactory().CreateAction("o", _ => { }));

            node.Should().BeOfType<CaseNode>();
            var branchingNode = (CaseNode)node;
            branchingNode.Choices.Should().HaveCount(4);
            branchingNode.Choices.Should().ContainKeys("key1", "key2", "key3", "");
        }

        [Fact]
        public async Task Run_MethodWithExistingKey_ExecutesCorrectBranch() {
            var node = CreateFactory().CreateCase("1",
                                             _ => "key2",
                                             new() {
                                                 ["key1"] = CreateFactory().CreateAction("k1", ctx => ctx["branch"] = "1"),
                                                 ["key2"] = CreateFactory().CreateAction("k2", ctx => ctx["branch"] = "2"),
                                                 ["key3"] = CreateFactory().CreateAction("k3", ctx => ctx["branch"] = "3"),
                                             });

            using var context = new Map();
            await node.Run(context);

            context["branch"].Should().Be("2");
        }

        [Fact]
        public async Task Run_MethodWithNonExistingKey_ThrowsInvalidOperationException() {
            var node = CreateFactory().CreateCase("1",
                                             _ => "nonexistent",
                                             new() {
                                                 ["key1"] = CreateFactory().CreateAction("k1", ctx => ctx["branch"] = "1"),
                                                 ["key2"] = CreateFactory().CreateAction("k2", ctx => ctx["branch"] = "2"),
                                             });
            var action = async () => {
                using var context = new Map();
                return await node.Run(context);
            };

            await action.Should().ThrowAsync<InvalidOperationException>()
                  .WithMessage("The path 'nonexistent' was not found.");
        }

        [Fact]
        public async Task Run_MethodWithNonExistingKeyAndWithOtherwise_ExecutesOtherwise() {
            var node = CreateFactory().CreateCase("1",
                                             _ => "nonexistent",
                                             new() {
                                                 ["key1"] = CreateFactory().CreateAction("k1", ctx => ctx["branch"] = "1"),
                                                 ["key2"] = CreateFactory().CreateAction("k2", ctx => ctx["branch"] = "2"),
                                             },
                                             CreateFactory().CreateAction("o", ctx => ctx["branch"] = "9"));
            using var context = new Map();
            await node.Run(context);

            context["branch"].Should().Be("9");
        }

        [Fact]
        public void CreateCase_ValidateMethod_ValidatesAllBranches() {
            var node = CreateFactory().CreateCase("1",
                                             _ => "key",
                                             new() {
                                                 ["key1"] = null,
                                                 ["key2"] = null,
                                                 ["key3"] = null,
                                             });

            var result = node.Validate();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}
