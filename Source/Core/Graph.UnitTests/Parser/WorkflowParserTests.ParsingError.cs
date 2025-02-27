namespace DotNetToolbox.Graph.Parser;

public partial class WorkflowParserTests {
    public class ParsingErrorTests : WorkflowParserTests {
        [Fact]
        public void Parse_InvalidToken_ReturnsErrorResult() {
            // Arrange
            const string script = "Invalid$Token";
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            var error = result.Errors.Should().ContainSingle().Subject;
            error.Message.Should().Be("Identifier can only contain letters, numbers, and underscores.");
            error.Sources.Should().BeEquivalentTo("Error@(1, 1): Identifier can only contain letters, numbers, and underscores.");
        }

        [Fact]
        public void Parse_MissingEndOfLine_ReturnsErrorResult() {
            // Arrange
            const string script = "Action1 Action2";
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            var error = result.Errors.Should().ContainSingle().Subject;
            error.Message.Should().Be("'EndOfLine' expected but found 'Identifier'.");
            error.Sources.Should().BeEquivalentTo("Identifier@(1, 9): Action2");
        }

        [Fact]
        public void Parse_IncompleteIf_ReturnsErrorResult() {
            // Arrange
            const string script = "IF Condition";
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            var error1 = result.Errors.First();
            error1.Message.Should().Be("Invalid indentation. Expected '1' but found 0.");
            error1.Sources.Should().BeEquivalentTo("EndOfFile@(1, 12)");
            var error2 = result.Errors.Skip(1).First();
            error2.Message.Should().Be("If statement must have a body.");
            error2.Sources.Should().BeEquivalentTo("EndOfFile@(1, 12)");
        }

        [Fact]
        public void Parse_InvalidCaseStructure_ReturnsErrorResult() {
            // Arrange
            const string script = """
                                  CASE Selection
                                    IS "Option1"
                                      Action1
                                    IS "Option2"
                                      Action2
                                    ELSE
                                      ActionDefault
                                  """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            var error1 = result.Errors.First();
            error1.Message.Should().Be("Unexpected token: 'Else'.");
            error1.Sources.Should().BeEquivalentTo("Else@(6, 3)");
            var error2 = result.Errors.Skip(1).First();
            error2.Message.Should().Be("Invalid indentation. Expected '0' but found 2.");
            error2.Sources.Should().BeEquivalentTo("Identifier@(7, 5): ActionDefault");
        }

        [Fact]
        public void Parse_MissingExitCode_ReturnsErrorResult() {
            // Arrange
            const string script = "EXIT InvalidCode";
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            var error1 = result.Errors.First();
            error1.Message.Should().Be("'EndOfLine' expected but found 'Identifier'.");
            error1.Sources.Should().BeEquivalentTo("Identifier@(1, 6): InvalidCode");
            var error2 = result.Errors.Skip(1).First();
            error2.Message.Should().Be("An exit node cannot be connected to another node.");
            error2.Sources.Should().BeEquivalentTo("Identifier@(1, 6): InvalidCode");
        }

        [Fact]
        public void Parse_InvalidJumpTarget_ReturnsErrorResult() {
            // Arrange
            const string script = "GOTO 123";
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(4);
            var error1 = result.Errors.First();
            error1.Message.Should().Be("'Identifier' expected but found 'Number'.");
            error1.Sources.Should().BeEquivalentTo("Number@(1, 6): 123");
            var error2 = result.Errors.Skip(1).First();
            error2.Message.Should().Be("'EndOfLine' expected but found 'Number'.");
            error2.Sources.Should().BeEquivalentTo("Number@(1, 6): 123");
            var error3 = result.Errors.Skip(2).First();
            error3.Message.Should().Be("Unexpected token: 'Number'.");
            error3.Sources.Should().BeEquivalentTo("Number@(1, 6): 123");
            var error4 = result.Errors.Skip(3).First();
            error4.Message.Should().Be("Jump target '123' not found.");
            error4.Sources.Should().BeEquivalentTo("GoTo@(1, 1)");
        }

        [Fact]
        public void Parse_MultipleErrors_ReturnsAllErrors() {
            // Arrange
            const string script = """
                                  InvalidToken
                                  IF Condition
                                    Action1
                                  CASE Selection
                                    IS "Option1"
                                      Action2
                                    ELSE
                                      ActionDefault
                                  """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
            var error1 = result.Errors.First();
            error1.Message.Should().Be("Unexpected token: 'Else'.");
            error1.Sources.Should().BeEquivalentTo("Else@(7, 3)");
            var error2 = result.Errors.Skip(1).First();
            error2.Message.Should().Be("Invalid indentation. Expected '0' but found 2.");
            error2.Sources.Should().BeEquivalentTo("Identifier@(8, 5): ActionDefault");
        }

        [Fact]
        public void Parse_EmptyCase_ReturnsErrorResult() {
            // Arrange
            const string script = """
                                  CASE Selection
                                  """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            var error1 = result.Errors.First();
            error1.Message.Should().Be("Invalid indentation. Expected '1' but found 0.");
            error1.Sources.Should().BeEquivalentTo("EndOfFile@(1, 14)");
        }

        [Fact]
        public void Parse_DuplicateOtherwise_ReturnsErrorResult() {
            // Arrange
            const string script = """
                                  CASE Selection
                                    IS "Option1"
                                      Action1
                                    OTHERWISE
                                      ActionDefault1
                                    OTHERWISE
                                      ActionDefault2
                                  """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(3);
            var error1 = result.Errors.First();
            error1.Message.Should().Be("'Otherwise' not allowed here.");
            error1.Sources.Should().BeEquivalentTo("Otherwise@(6, 3)");
            var error2 = result.Errors.Skip(1).First();
            error2.Message.Should().Be("Unexpected token: 'Otherwise'.");
            error2.Sources.Should().BeEquivalentTo("Otherwise@(6, 3)");
            var error3 = result.Errors.Skip(2).First();
            error3.Message.Should().Be("Invalid indentation. Expected '0' but found 2.");
            error3.Sources.Should().BeEquivalentTo("Identifier@(7, 5): ActionDefault2");
        }

        [Fact]
        public void Parse_InvalidIndentation_ReturnsErrorResult() {
            // Arrange
            const string script = """
                                  IF Condition
                                  Action1
                                  """;
            var tokens = WorkflowLexer.Tokenize(script).ToList();

            // Act
            var result = WorkflowParser.Parse(tokens, _services);

            // Assert
            result.IsSuccessful.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            var error1 = result.Errors.First();
            error1.Message.Should().Be("Invalid indentation. Expected '1' but found 0.");
            error1.Sources.Should().BeEquivalentTo("Identifier@(2, 1): Action1");
        }
    }
}
