namespace DotNetToolbox.Graph.Parser;

public partial class WorkflowParserTests {
    public class ControlStructureTests : WorkflowParserTests {
        [Fact]
        public void Parse_IfThen_ReturnsWorkflowWithIfStructure() {
            // Arrange
            const string script = """
                              IF Condition
                                ActionTrue
                              """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var ifNode = result.Value.Should().BeOfType<IfNode>().Subject;
            ifNode.Id.Should().Be(1);
            ifNode.Tag.Should().BeNull();
            ifNode.Label.Should().Be("Condition");
            ifNode.Next.Should().BeNull();

            var trueAction = ifNode.Then.Should().BeOfType<ActionNode>().Subject;
            trueAction.Id.Should().Be(2);
            trueAction.Tag.Should().BeNull();
            trueAction.Name.Should().Be("ActionTrue");

            trueAction.Next.Should().BeNull();
            ifNode.Else.Should().BeNull();
        }

        [Fact]
        public void Parse_IfThenWithExit_ReturnsWorkflowWithIfStructure() {
            // Arrange
            const string script = """
                              IF Condition
                                ActionTrue
                              EXIT
                              """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var ifNode = result.Value.Should().BeOfType<IfNode>().Subject;
            ifNode.Id.Should().Be(1);
            ifNode.Label.Should().Be("Condition");

            var trueAction = ifNode.Then.Should().BeOfType<ActionNode>().Subject;
            trueAction.Id.Should().Be(2);
            trueAction.Name.Should().Be("ActionTrue");

            var end = trueAction.Next.Should().BeOfType<ExitNode>().Subject;
            end.Id.Should().Be(3);
            end.Label.Should().Be("exit");
            end.ExitCode.Should().Be(0);

            ifNode.Else.Should().Be(end);
        }

        [Fact]
        public void Parse_IfThenElse_ReturnsWorkflowWithIfElseStructure() {
            // Arrange
            const string script = """
                              IF Condition
                                ActionTrue
                              ELSE
                                ActionFalse
                              Action1
                              """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeTrue();
            var ifNode = result.Value.Should().BeOfType<IfNode>().Subject;
            ifNode.Id.Should().Be(1);
            ifNode.Label.Should().Be("Condition");

            var trueAction = ifNode.Then.Should().BeOfType<ActionNode>().Subject;
            trueAction.Id.Should().Be(2);
            trueAction.Name.Should().Be("ActionTrue");

            var falseAction = ifNode.Else.Should().BeOfType<ActionNode>().Subject;
            falseAction.Id.Should().Be(3);
            falseAction.Name.Should().Be("ActionFalse");

            var endTrue = trueAction.Next.Should().BeOfType<ActionNode>().Subject;
            endTrue.Id.Should().Be(4);
            endTrue.Name.Should().Be("Action1");

            falseAction.Next.Should().Be(endTrue);
        }

        [Fact]
        public void Parse_Case_ReturnsWorkflowWithCaseStructure() {
            // Arrange
            const string script = """
                              CASE Selection
                                IS "Option1"
                                  Action1
                                IS "Option2"
                                  Action2
                                OTHERWISE
                                  ActionDefault
                              EXIT
                              """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.Errors.Should().BeEmpty();
            result.IsSuccessful.Should().BeTrue();
            var caseNode = result.Value.Should().BeOfType<CaseNode>().Subject;
            caseNode.Id.Should().Be(1);
            caseNode.Label.Should().Be("Selection");

            caseNode.Choices.Should().HaveCount(3);

            var option1 = caseNode.Choices["Option1"].Should().BeOfType<ActionNode>().Subject;
            option1.Id.Should().Be(2);
            option1.Name.Should().Be("Action1");

            var option2 = caseNode.Choices["Option2"].Should().BeOfType<ActionNode>().Subject;
            option2.Id.Should().Be(3);
            option2.Name.Should().Be("Action2");

            var otherwise = caseNode.Choices[string.Empty].Should().BeOfType<ActionNode>().Subject;
            otherwise.Id.Should().Be(4);
            otherwise.Name.Should().Be("ActionDefault");

            var end = option1.Next.Should().BeOfType<ExitNode>().Subject;
            end.Id.Should().Be(5);
            end.Label.Should().Be("exit");
            end.ExitCode.Should().Be(0);

            option2.Next.Should().Be(end);
            otherwise.Next.Should().Be(end);
        }
    }
}
