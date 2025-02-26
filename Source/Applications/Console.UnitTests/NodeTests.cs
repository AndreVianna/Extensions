namespace DotNetToolbox.ConsoleApplication;

public sealed class NodeTests {
    [Fact]
    public void Constructor_CreatesNode() {
        // Arrange & Act
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);

        // Act
        var node = new TestNode(app, "node", ["n"]) {
            Description = "Some description.",
        };

        // Assert
        node.Name.Should().Be("node");
        node.Help.Should().Be("Some description.");
        node.Description.Should().Be("Some description.");
        node.Aliases.Should().BeEquivalentTo("n");
        node.Parent.Should().Be(app);
        node.Application.Should().Be(app);
        node.Children.Should().BeEmpty();
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]) {
            Help = "Some help.",
            Description = "Some description.",
        };
        var expectedToString = $"TestNode: {node.Name}, {node.Aliases[0]} => {node.Help}";

        // Act
        var actualToString = node.ToString();

        // Assert
        actualToString.Should().Be(expectedToString);
        node.Help.Should().Be("Some help.");
        node.Description.Should().Be("Some description.");
    }

    private sealed class InvalidCommandDelegates : TheoryData<Delegate> {
        public InvalidCommandDelegates() {
            Add(() => 13);
            Add((Command _) => "Invalid");
            Add((string _) => { });
        }
    }
    [Theory]
    [ClassData(typeof(InvalidCommandDelegates))]
    public void AddCommand_WithInvalidDelegate_AddsCommand(Delegate action) {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        var result = () => node.AddCommand("command", action);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task AddCommand_WithException_AddsCommandThatThrows() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);
        var command = (Command)node.AddCommand("command", (Action)(() => throw new()));

        // Act
        var result = () => command.Execute([]);

        // Assert
        await result.Should().ThrowAsync<Exception>();
    }

    private sealed class CommandDelegates : TheoryData<Delegate> {
        public CommandDelegates() {
            Add(null!);
            Add(() => { });
            Add((Command _) => { });
            Add(() => Result.Success());
            Add((Command _) => Result.Success());
            Add(() => Result.Task.FromResult(Success()));
            Add((Command _) => Result.Task.FromResult(Success()));
            Add(() => Task.CompletedTask);
            Add((Command _) => Task.CompletedTask);
            Add((CancellationToken _) => Result.Task.FromResult(Success()));
            Add((Command _, CancellationToken _) => Result.Task.FromResult(Success()));
            Add((CancellationToken _) => Task.CompletedTask);
            Add((Command _, CancellationToken _) => Task.CompletedTask);
        }
    }
    [Theory]
    [ClassData(typeof(CommandDelegates))]
    public async Task AddCommand_AddsCommand(Delegate action) {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        var subject = node.AddCommand("command", action);

        // Assert
        node.Children.Should().ContainSingle(x => x.Name == "command");
        var command = subject.Should().BeOfType<Command>().Subject;
        command.Aliases.Should().BeEmpty();
        var result = await command.Execute([]);
        result.Should().Be(Result.Success());
    }

    [Theory]
    [ClassData(typeof(CommandDelegates))]
    public void AddCommand_WithAlias_AddsCommand(Delegate action) {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        node.AddCommand("command", "c", action);

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == "command").Subject;
        child.Aliases.Should().BeEquivalentTo("c");
    }

    [Fact]
    public async Task AddCommand_OfType_AddsCommandOfType() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        node.AddCommand<TestCommand>();

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == "Command").Subject;
        var command = child.Should().BeOfType<TestCommand>().Subject;
        command.Aliases.Should().BeEquivalentTo("c");
        var text = command.ToString();
        text.Should().Be("TestCommand: Command, c => Test command.");
        var result = () => command.Execute([]);
        await result.Should().NotThrowAsync();
    }

    [Fact]
    public void AddCommand_WithCommand_AddsCommand() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var parent = new TestNode(app, "node");
        var node = new TestCommand(parent);

        // Act
        parent.AddCommand(node);

        // Assert
        var child = parent.Children.Should().ContainSingle(x => x.Name == "Command").Subject;
        child.Should().BeOfType<TestCommand>();
    }

    [Fact]
    public void AddCommand_ManyTimes_AddsMultipleCommand() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var parent = new TestNode(app, "node");

        // Act
        parent.AddCommand("Node1", () => { });
        parent.AddCommand("Node2", () => { });

        // Assert
        parent.Commands.Should().HaveCount(2);
    }

    [Fact]
    public void AddOption_AddsOption() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        node.AddOption("option");

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == "option").Subject;
        var option = child.Should().BeOfType<Option>().Subject;
        option.Aliases.Should().BeEmpty();
    }

    [Fact]
    public void AddOption_WithAlias_AddsOption() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        node.AddOption("option", "o");

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == "option").Subject;
        var option = child.Should().BeOfType<Option>().Subject;
        option.Aliases.Should().BeEquivalentTo("o");
    }

    [Fact]
    public void AddOption_OfType_AddsOptionOfType() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        node.AddOption<TestOption>();

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == "MultipleChoiceOption").Subject;
        var option = child.Should().BeOfType<TestOption>().Subject;
        option.Aliases.Should().BeEquivalentTo("o");
    }

    [Fact]
    public void AddOption_WithOption_AddsOption() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var parent = new TestNode(app, "node");
        var node = new TestOption(parent);

        // Act
        parent.AddOption(node);

        // Assert
        var child = parent.Children.Should().ContainSingle(x => x.Name == "MultipleChoiceOption").Subject;
        child.Should().BeOfType<TestOption>();
    }

    [Fact]
    public void AddParameter_AddsParameter() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);
        const string parameterName = "param1";

        // Act
        node.AddParameter(parameterName);

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == parameterName).Subject;
        var parameter = child.Should().BeOfType<Parameter>().Subject;
        parameter.Order.Should().Be(0);
        parameter.IsRequired.Should().BeTrue();
        parameter.IsSet.Should().BeFalse();
    }

    [Fact]
    public void AddParameter_WithDefaultValue_AddsParameter() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);
        const string parameterName = "param1";
        const string defaultValue = "defaultValue";

        // Act
        node.AddParameter(parameterName, defaultValue);

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == parameterName).Subject;
        var parameter = child.Should().BeOfType<Parameter>().Subject;
        parameter.Order.Should().Be(0);
        parameter.IsRequired.Should().BeFalse();
        parameter.IsSet.Should().BeFalse();
    }

    [Fact]
    public void AddParameter_OfType_AddsParameterOfType() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        node.AddParameter<TestParameter>();

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == "Age").Subject;
        var parameter = child.Should().BeOfType<TestParameter>().Subject;
        parameter.Aliases.Should().BeEmpty();
        parameter.Order.Should().Be(0);
    }

    [Fact]
    public void AddParameter_WithParameter_AddsParameter() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var parent = new TestNode(app, "node");
        var node = new TestParameter(parent);

        // Act
        parent.AddParameter(node);

        // Assert
        var child = parent.Children.Should().ContainSingle(x => x.Name == "Age").Subject;
        child.Should().BeOfType<TestParameter>();
    }

    private sealed class InvalidFlagDelegates : TheoryData<Delegate> {
        public InvalidFlagDelegates() {
            Add(() => 13);
            Add((Flag _) => "Invalid");
            Add((string _) => { });
        }
    }
    [Theory]
    [ClassData(typeof(InvalidFlagDelegates))]
    public void AddFlag_WithInvalidDelegate_AddsFlag(Delegate action) {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        var result = () => node.AddFlag("flag", action);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task AddFlag_WithException_AddsFlagThatThrows() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);
        var flag = node.AddFlag("flag", (Action)(() => throw new()));
        var context = new Map();

        // Act
        var result = () => flag.Read(context);

        // Assert
        await result.Should().ThrowAsync<Exception>();
    }

    private sealed class FlagDelegates : TheoryData<Delegate> {
        public FlagDelegates() {
            Add(null!);
            Add(() => { });
            Add((Flag _) => { });
            Add(() => Result.Success());
            Add((Flag _) => Result.Success());
            Add(() => Result.Task.FromResult(Success()));
            Add((Flag _) => Result.Task.FromResult(Success()));
            Add(() => Task.CompletedTask);
            Add((Flag _) => Task.CompletedTask);
            Add((CancellationToken _) => Result.Task.FromResult(Success()));
            Add((Flag _, CancellationToken _) => Result.Task.FromResult(Success()));
            Add((CancellationToken _) => Task.CompletedTask);
            Add((Flag _, CancellationToken _) => Task.CompletedTask);
        }
    }
    [Theory]
    [ClassData(typeof(FlagDelegates))]
    public async Task AddFlag_AddsFlag(Delegate action) {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);
        var context = new Map();

        // Act
        var subject = node.AddFlag("flag", action);

        // Assert
        node.Children.Should().ContainSingle(x => x.Name == "flag");
        var flag = subject.Should().BeOfType<Flag>().Subject;
        flag.Aliases.Should().BeEmpty();
        var result = await subject.Read(context);
        result.Should().Be(Result.Success());
    }

    [Theory]
    [ClassData(typeof(FlagDelegates))]
    public void AddFlag_WithAlias_AddsFlag(Delegate action) {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        node.AddFlag("flag", "c", action);

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == "flag").Subject;
        child.Aliases.Should().BeEquivalentTo("c");
    }

    [Fact]
    public void AddFlag_OfType_AddsFlagOfType() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var node = new TestNode(app, "node", ["n"]);

        // Act
        node.AddFlag<TestFlag>();

        // Assert
        var child = node.Children.Should().ContainSingle(x => x.Name == "Flag").Subject;
        var flag = child.Should().BeOfType<TestFlag>().Subject;
        flag.Aliases.Should().BeEquivalentTo("f");
    }

    [Fact]
    public void AddFlag_WithFlag_AddsFlag() {
        // Arrange
        var app = Substitute.For<IApplication>();
        var serviceProvider = CreateFakeServiceProvider();
        app.Services.Returns(serviceProvider);
        var parent = new TestNode(app, "node");
        var node = new TestFlag(parent);

        // Act
        parent.AddFlag(node);

        // Assert
        var child = parent.Children.Should().ContainSingle(x => x.Name == "Flag").Subject;
        child.Should().BeOfType<TestFlag>();
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestCommand
        : Command<TestCommand> {
        public TestCommand(IHasChildren app)
            : base(app, "Command", n => n.Aliases = ["c"]) {
            Description = "Test command.";
        }

        protected override Task<Result> ExecuteAsync(CancellationToken ct = default) {
            Logger.LogInformation("Some logger.");
            return base.ExecuteAsync(ct);
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestOption(IHasChildren app) : Option<TestOption>(app, "MultipleChoiceOption", n => n.Aliases = ["o"]);
    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestParameter(IHasChildren app) : Parameter<TestParameter>(app, "Age", n => n.DefaultValue = "18");
    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestFlag(IHasChildren app) : Flag<TestFlag>(app, "Flag", n => n.Aliases = ["f"]);

    private readonly IAssemblyDescriptor _assemblyDescriptor = Substitute.For<IAssemblyDescriptor>();
    private readonly IAssemblyAccessor _assemblyAccessor = Substitute.For<IAssemblyAccessor>();
    private IServiceProvider CreateFakeServiceProvider() {
        var output = new TestOutput();
        var input = new TestInput(output);
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(IConfiguration)).Returns(Substitute.For<IConfiguration>());
        serviceProvider.GetService(typeof(IOutput)).Returns(output);
        serviceProvider.GetService(typeof(IInput)).Returns(input);
        _assemblyAccessor.GetEntryAssembly().Returns(_assemblyDescriptor);
        _assemblyDescriptor.Name.Returns("TestApp");
        _assemblyDescriptor.Version.Returns(new Version(1, 0));
        serviceProvider.GetService(typeof(IAssemblyAccessor)).Returns(_assemblyAccessor);
        serviceProvider.GetService(typeof(IDateTimeProvider)).Returns(Substitute.For<IDateTimeProvider>());
        serviceProvider.GetService(typeof(IGuidProvider)).Returns(Substitute.For<IGuidProvider>());
        serviceProvider.GetService(typeof(IFileSystemAccessor)).Returns(Substitute.For<IFileSystemAccessor>());
        serviceProvider.GetService(typeof(ILoggerFactory)).Returns(Substitute.For<ILoggerFactory>());
        return serviceProvider;
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestNode(IHasChildren parent, string name, params string[] aliases)
        : Command<TestNode>(parent, name, n => n.Aliases = aliases);
}
