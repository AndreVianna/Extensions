namespace DotNetToolbox.ConsoleApplication.Utilities;

public class ArgumentsParserTests {
    private readonly IApplication _app = Substitute.For<IApplication>();
    private readonly IServiceProvider _serviceProvider = Substitute.For<IServiceProvider>();
    private readonly ILoggerFactory _loggerFactory = Substitute.For<ILoggerFactory>();
    private readonly IFlag _flag;
    private readonly IOption _option;
    private readonly IParameter _parameter;
    private readonly IParameter _requiredParameter;
    private readonly Command _command;

    public ArgumentsParserTests() {
        _serviceProvider.GetService(typeof(ILoggerFactory)).Returns(_loggerFactory);
        _app.Services.Returns(_serviceProvider);
        _app.Context.Returns(new Map());
        _option = new Option(_app, "Option", o => o.Aliases = ["o"]);
        _flag = new Flag(_app, "Flag", f => f.Aliases = ["f"]);
        _requiredParameter = new Parameter(_app, "Name");
        _parameter = new Parameter(_app, "Age", p => p.DefaultValue = "18");
        _command = new(_app, "Say", c => c.Aliases = ["s"], (_, _) => Result.Task.FromResult(Success()));
    }

    [Fact]
    public async Task Read_WithNoArguments_ShouldReturnSuccess() {
        // Arrange & Act
        var result = await ArgumentsParser.Parse(_app, [], default);

        // Assert
        result.Should().BeEquivalentTo(Result.Success());
    }

    [Fact]
    public async Task Read_WithInvalidArgument_ReturnsInvalid() {
        // Arrange
        var arguments = new[] { "unknown" };
        const string expectedMessage = "Unknown argument 'unknown'. For a list of available arguments use '--help'.";

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsInvalid.Should().BeTrue();
        result.Errors[0].Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Read_WithInvalidOption_ReturnsInvalid() {
        // Arrange
        var arguments = new[] { "--unknown" };
        const string expectedMessage = "Unknown argument '--unknown'. For a list of available arguments use '--help'.";

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsInvalid.Should().BeTrue();
        result.Errors[0].Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Read_WithInvalidAlias_ReturnsInvalid() {
        // Arrange
        var arguments = new[] { "-u" };
        const string expectedMessage = "Unknown argument '-u'. For a list of available arguments use '--help'.";

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsInvalid.Should().BeTrue();
        result.Errors[0].Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Read_WithFlagByName_AndValue_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "--flag" };
        _app.Children.Returns([_flag]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Flag"].Should().BeOfType<string>().Subject.Should().Be("True");
    }

    [Fact]
    public async Task Read_WithFlagByAlias_AndValue_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "-f" };
        _app.Children.Returns([_flag]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Flag"].Should().BeOfType<string>().Subject.Should().Be("True");
    }

    [Fact]
    public async Task Read_WithOptionByName_WithoutValue_ReturnsInvalid() {
        // Arrange
        var arguments = new[] { "--option" };
        _app.Children.Returns([_option]);
        const string expectedMessage = "Missing value for option '--option'.";

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsInvalid.Should().BeTrue();
        result.Errors[0].Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Read_WithOptionByName_AndValue_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "--option", "42" };
        _app.Children.Returns([_option]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Option"].Should().BeOfType<string>().Subject.Should().Be("42");
    }

    [Fact]
    public async Task Read_WithOptionByAlias_AndValue_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "-o", "42" };
        _app.Children.Returns([_option]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Option"].Should().BeOfType<string>().Subject.Should().Be("42");
    }

    [Fact]
    public async Task Read_WithOptionByName_AndQuotedValue_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "-o", """
                                      "John Doe"
                                      """ };
        _app.Children.Returns([_option]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Option"].Should().Be("John Doe");
    }

    [Fact]
    public async Task Read_WithOptionByName_AndDefaultKeyWord_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "-o", "default" };
        _app.Children.Returns([_option]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Option"].Should().BeNull();
    }

    [Fact]
    public async Task Read_WithOptionByAlias_AndNullKeyWord_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "-o", "null" };
        _app.Children.Returns([_option]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Option"].Should().BeNull();
    }

    [Fact]
    public async Task Read_WithValueForParameter_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "42" };
        _app.Children.Returns([_parameter]);
        _app.Parameters.Returns([_parameter]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Age"].Should().BeOfType<string>().Subject.Should().Be("42");
    }

    [Fact]
    public async Task Read_WithValueForRequiredParameterOnly_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "John Doe" };
        _app.Children.Returns([_requiredParameter, _parameter]);
        _app.Parameters.Returns([_requiredParameter, _parameter]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Name"].Should().Be("John Doe");
        _app.Context["Age"].Should().Be("18");
    }

    [Fact]
    public async Task Read_WithOptionAfterArgument_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "42", "-o" };
        _app.Children.Returns([_requiredParameter, _parameter]);
        _app.Parameters.Returns([_requiredParameter, _parameter]);
        const string expectedMessage = "Unknown argument '-o'. For a list of available arguments use '--help'.";

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsInvalid.Should().BeTrue();
        result.Errors[0].Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Read_WithNoValueForOptionalParameter_ReturnsSuccess() {
        // Arrange
        var arguments = Array.Empty<string>();
        _app.Children.Returns([_parameter]);
        _app.Parameters.Returns([_parameter]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Age"].Should().BeOfType<string>().Subject.Should().Be("18");
    }

    [Fact]
    public async Task Read_WithDefaultKeyWordForParameter_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "default" };
        _app.Children.Returns([_parameter]);
        _app.Parameters.Returns([_parameter]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Age"].Should().BeOfType<string>().Subject.Should().Be("18");
    }

    [Fact]
    public async Task Read_WithNullKeyWordForParameter_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "null" };
        _app.Children.Returns([_parameter]);
        _app.Parameters.Returns([_parameter]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Age"].Should().BeNull();
    }

    [Fact]
    public async Task Read_WithOption_AndNoValueForRequiredParameter_ReturnsInvalid() {
        // Arrange
        var arguments = new[] { "-o", """
                                      "John Doe"
                                      """ };
        _app.Children.Returns([_option, _requiredParameter]);
        _app.Options.Returns([_option]);
        _app.Parameters.Returns([_requiredParameter]);
        const string expectedMessage = "Required parameter is missing: 'Name'.";

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsInvalid.Should().BeTrue();
        result.Errors[0].Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Read_WithFullSetOfArguments_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "--flag", "-o", "42", """
                                                      "John Doe"
                                                      """, "20" };
        _app.Children.Returns([_flag, _option, _requiredParameter, _parameter]);
        _app.Options.Returns([_flag, _option]);
        _app.Parameters.Returns([_requiredParameter, _parameter]);
        _app.Context.Returns(new Map());

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        _app.Context["Flag"].Should().BeOfType<string>().Subject.Should().Be("True");
        _app.Context["Option"].Should().BeOfType<string>().Subject.Should().Be("42");
        _app.Context["Name"].Should().BeOfType<string>().Subject.Should().Be("John Doe");
        _app.Context["Age"].Should().BeOfType<string>().Subject.Should().Be("20");
    }

    [Fact]
    public async Task Read_WithNoValueForRequiredParameter_ReturnsInvalid() {
        // Arrange
        var arguments = Array.Empty<string>();
        _app.Children.Returns([_requiredParameter]);
        _app.Parameters.Returns([_requiredParameter]);
        const string expectedMessage = "Required parameter is missing: 'Name'.";

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsInvalid.Should().BeTrue();
        result.Errors[0].Message.Should().Be(expectedMessage);
    }

    [Fact]
    public async Task Read_WithCommandByName_ReturnsInvalid() {
        // Arrange
        var arguments = new[] { "say" };
        _app.Children.Returns([_command]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task Read_WithCommandByAlias_ReturnsInvalid() {
        // Arrange
        var arguments = new[] { "s" };
        _app.Children.Returns([_command]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task Read_WithCommand_WithSubCommand_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "s", "-f" };
        _app.Children.Returns([_command]);
        _command.AddFlag("Flag", "f");
        _command.AddCommand("TheCommand", () => { });
        _command.AddFlag("ACommand", () => { });
        _command.AddParameter("First", "1");
        _command.AddParameter("Second", "2");

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task Read_WithCommandWithOption_AndValue_ReturnsSuccess() {
        // Arrange
        var arguments = new[] { "-o", "42" };
        _app.Children.Returns([_option]);

        // Act
        var result = await ArgumentsParser.Parse(_app, arguments, default);

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
