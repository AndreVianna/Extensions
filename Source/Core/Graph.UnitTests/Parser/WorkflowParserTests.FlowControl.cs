namespace DotNetToolbox.Graph.Parser;

public partial class WorkflowParserTests {
    public class FlowControlTests : WorkflowParserTests {
        [Fact]
        public void Parse_ExitWithCode_ReturnsWorkflowWithExitCode() {
            // Arrange
            const string script = """
                              DoSomething
                              EXIT 13
                              """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var start = result.Value.Should().BeOfType<ActionNode>().Subject;
            start.Id.Should().Be(1);
            start.Name.Should().Be("DoSomething");

            var end = start.Next.Should().BeOfType<ExitNode>().Subject;
            end.Id.Should().Be(2);
            end.Label.Should().Be("exit");
            end.ExitCode.Should().Be(13);
        }

        [Fact]
        public void Parse_JumpTo_ReturnsWorkflowWithJump() {
            // Arrange
            const string script = """
                              Action1 :Label1:
                              Action2
                              IF Condition
                                GOTO end
                              GOTO Label1
                              EXIT :end:
                              """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var action1 = result.Value.Should().BeOfType<ActionNode>().Subject;
            action1.Tag.Should().Be("Label1");
            action1.Name.Should().Be("Action1");

            var action2 = action1.Next.Should().BeOfType<ActionNode>().Subject;
            action2.Id.Should().Be(2);
            action2.Name.Should().Be("Action2");

            var ifNode = action2.Next.Should().BeOfType<IfNode>().Subject;
            ifNode.Id.Should().Be(3);
            ifNode.Label.Should().Be("Condition");

            var exitJump = ifNode.Then.Should().BeOfType<JumpNode>().Subject;
            exitJump.Id.Should().Be(4);
            exitJump.Label.Should().Be("goto");
            exitJump.TargetTag.Should().Be("end");

            var jumpBack = ifNode.Else.Should().BeOfType<JumpNode>().Subject;
            jumpBack.Id.Should().Be(5);
            jumpBack.Label.Should().Be("goto");
            jumpBack.TargetTag.Should().Be("Label1");
            jumpBack.Next.Should().Be(action1);

            var end = exitJump.Next.Should().BeOfType<ExitNode>().Subject;
            end.Tag.Should().Be("end");
            end.Label.Should().Be("exit");
            end.ExitCode.Should().Be(0);
        }
    }
}
