namespace DotNetToolbox.Graph.Parser;

public partial class WorkflowParserTests {
    public class BasicFunctionalityTests : WorkflowParserTests {
        [Fact]
        public void Parse_EmptyInput_ReturnsEmptyWorkflow() {
            // Arrange
            const string script = "";
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            result.Value.Should().BeNull();
        }

        [Fact]
        public void Parse_SingleAction_ReturnsWorkflowWithOneAction() {
            // Arrange
            const string script = """
                                  DoSomething
                                  """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var start = result.Value.Should().BeOfType<ActionNode>().Subject;
            start.Id.Should().Be(1);
            start.Tag.Should().BeNull();
            start.Name.Should().Be("DoSomething");
            start.Next.Should().BeNull();
        }

        [Fact]
        public void Parse_SingleActionWithExit_ReturnsWorkflowWithOneAction() {
            // Arrange
            const string script = """
                                  DoSomething
                                  EXIT
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
            end.Tag.Should().BeNull();
            end.Label.Should().Be("exit");
            end.ExitCode.Should().Be(0);
        }

        [Fact]
        public void Parse_MultipleActions_ReturnsWorkflowWithMultipleActions() {
            // Arrange
            const string script = """
                                  Action1
                                  Action2
                                  Action3
                                  """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var action1 = result.Value.Should().BeOfType<ActionNode>().Subject;
            action1.Id.Should().Be(1);
            action1.Name.Should().Be("Action1");
            var action2 = action1.Next.Should().BeOfType<ActionNode>().Subject;
            action2.Id.Should().Be(2);
            action2.Name.Should().Be("Action2");
            var action3 = action2.Next.Should().BeOfType<ActionNode>().Subject;
            action3.Id.Should().Be(3);
            action3.Name.Should().Be("Action3");
            action3.Next.Should().BeNull();
        }

        [Fact]
        public void Parse_ActionWithLabelAndTag_ReturnsWorkflowWithLabeledAndTaggedAction() {
            // Arrange
            const string script = """
                                  DoSomething :Tag:
                                  """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var start = result.Value.Should().BeOfType<ActionNode>().Subject;
            start.Id.Should().Be(1);
            start.Tag.Should().Be("Tag");
            start.Name.Should().Be("DoSomething");
            start.Next.Should().BeNull();
        }
    }
}
