namespace DotNetToolbox.ConsoleApplication.Utilities;
public class OutputFormatterTests {
    [Fact]
    public void FormatException_WithSingleException_IncludesTypeAndMessage() {
        // Arrange
        var exception = new InvalidOperationException("Test exception");

        // Act
        var result = exception.ToText();

        // Assert
        result.Should().Contain("InvalidOperationException");
        result.Should().Contain("Test exception");
    }

    [Fact]
    public void FormatException_WithInnerException_IncludesBothExceptions() {
        // Arrange
        var innerException = new InvalidOperationException("Inner exception");
        var outerException = new Exception("Outer exception", innerException);
        const string expectedResult = """
                                      Exception: Outer exception
                                          Inner Exception => InvalidOperationException: Inner exception

                                      """;

        // Act
        var result = outerException.ToText();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatValidationErrors_WithNoErrors_ReturnsEmptyString() {
        // Arrange
        var errors = Array.Empty<Error>();

        // Act
        var result = errors.ToText();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FormatValidationErrors_WithMultipleErrors_FormatsAllErrors() {
        // Arrange
        var errors = new List<Error> {
            new("First error."),
            new("Second error."),
        };
        const string expectedResult = """
                                      Validation error: First error.
                                      Validation error: Second error.

                                      """;

        // Act
        var result = errors.ToText();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_WithNode_IncludesDescriptionAndUsage() {
        // Arrange
        var node = Substitute.For<IHasChildren>();
        node.Name.Returns("test-command");
        node.Path.Returns("test-command");
        node.Help.Returns("Test command description.");
        const string expectedResult = """
                                      Test command description.

                                      Usage:
                                          test-command

                                      """;

        // Act
        var result = node.ToHelp();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_WithApplication_IncludesFullNameAndVersion() {
        // Arrange
        var app = Substitute.For<IApplication>();
        app.FullName.Returns("TestApp");
        app.Version.Returns("1.0.0");
        app.Path.Returns("TestApp");
        const string expectedResult = """
                                      TestApp

                                      Usage:
                                          TestApp

                                      """;

        // Act
        var result = app.ToHelp();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_WithChildren_IncludesSectionHeaders() {
        // Arrange
        var node = Substitute.For<IHasChildren>();
        node.Name.Returns("test-command");
        node.Path.Returns("test-command");
        node.Help.Returns("Test command description.");
        var option = Substitute.For<IOption>();
        option.Name.Returns("option");
        option.Help.Returns("Description for option.");
        node.Options.Returns([option]);
        var parameter = Substitute.For<IParameter>();
        parameter.Name.Returns("parameter");
        parameter.Help.Returns($"Description for parameter.{System.Environment.NewLine}");
        node.Parameters.Returns([parameter]);
        var command = Substitute.For<ICommand>();
        command.Name.Returns("subcommand");
        command.Help.Returns($"Description for subcommand line1.{System.Environment.NewLine}Description for subcommand line2.{System.Environment.NewLine}");
        node.Commands.Returns([command]);
        node.Children.Returns([option, parameter, command]);

        const string expectedResult = """
                                  Test command description.

                                  Usage:
                                      test-command [Options] [Commands]
                                      test-command [Options] [<parameter>]

                                  Options:
                                      --option                  Description for option.

                                  Parameters:
                                      parameter                 Description for parameter.

                                  Commands:
                                      subcommand                Description for subcommand line1.
                                                                Description for subcommand line2.

                                  """;

        // Act
        var result = node.ToHelp();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_WithEmptyChildren_DoesNotIncludeSectionHeaders() {
        // Arrange
        var node = Substitute.For<IHasChildren>();
        node.Name.Returns("test-command");
        node.Path.Returns("test-command");
        node.Help.Returns("Test command description.");
        node.Options.Returns([]);
        node.Parameters.Returns([]);
        node.Commands.Returns([]);

        const string expectedResult = """
                                  Test command description.

                                  Usage:
                                      test-command

                                  """;

        // Act
        var result = node.ToHelp();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_ForApplicationWithChildren_IncludesSectionHeaders() {
        // Arrange
        var node = Substitute.For<IApplication>();
        node.FullName.Returns("TestApp v1.0");
        node.AssemblyName.Returns("test-command");
        node.Path.Returns("test-command");
        node.Help.Returns("Test command description.");
        var option = Substitute.For<IOption>();
        option.Name.Returns("option");
        option.Help.Returns("Description for option.");
        node.Options.Returns([option]);
        var parameter = Substitute.For<IParameter>();
        parameter.Name.Returns("parameter");
        parameter.IsRequired.Returns(true);
        parameter.Help.Returns("Description for parameter.");
        node.Parameters.Returns([parameter]);
        var command = Substitute.For<ICommand>();
        command.Name.Returns("subcommand");
        command.Help.Returns("Description for subcommand.");
        node.Commands.Returns([command]);
        node.Children.Returns([option, parameter, command]);

        const string expectedResult = """
                                      TestApp v1.0
                                      Test command description.

                                      Usage:
                                          test-command [Options] [Commands]
                                          test-command [Options] <parameter>

                                      Options:
                                          --option                  Description for option.

                                      Parameters:
                                          parameter                 Description for parameter.

                                      Commands:
                                          subcommand                Description for subcommand.

                                      """;

        // Act
        var result = node.ToHelp();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatException_WithoutStackTrace_IncludesExceptionWithoutStackTrace() {
        // Arrange
        var exception = new InvalidOperationException("Test exception without stack trace");
        const string expectedResult = """
                                  InvalidOperationException: Test exception without stack trace

                                  """;

        // Act
        var result = exception.ToText();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_WithNodeHavingAliases_IncludesAliasesInDescription() {
        // Arrange
        var node = Substitute.For<IHasChildren>();
        node.Path.Returns("test-command");
        node.Name.Returns("test-command");
        node.Aliases.Returns(["t", "test"]);
        node.Help.Returns("Test command with aliases.");
        const string expectedResult = """
                                  Test command with aliases.

                                  Usage:
                                      test-command

                                  Aliases: t, test

                                  """;

        // Act
        var result = node.ToHelp();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_WithIncludeApplicationTrue_IncludesApplicationName() {
        // Arrange
        var app = Substitute.For<IApplication>();
        app.FullName.Returns("TestApp");
        app.Help.Returns("Application description.");
        app.Path.Returns("TestApp");
        const string expectedResult = """
                                  TestApp
                                  Application description.

                                  Usage:
                                      TestApp

                                  """;

        // Act
        var result = app.ToHelp();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_WithNodeHavingWhitespaceInDescription_TrimsDescription() {
        // Arrange
        var node = Substitute.For<IHasChildren>();
        node.Path.Returns("test-command");
        node.Help.Returns("  Test command with leading and trailing whitespace.  ");
        const string expectedResult = """
                                  Test command with leading and trailing whitespace.

                                  Usage:
                                      test-command

                                  """;

        // Act
        var result = node.ToHelp();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_WithNodeHavingEmptyDescription_DoesNotIncludeEmptyDescription() {
        // Arrange
        var node = Substitute.For<IHasChildren>();
        node.Path.Returns("test-command");
        node.Help.Returns("");
        const string expectedResult = """
                                  Usage:
                                      test-command

                                  """;

        // Act
        var result = node.ToHelp();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void FormatHelp_WithNullNode_ThrowsArgumentNullException() {
        // Act
        var act = () => ((IHasChildren)null!).ToHelp();

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
