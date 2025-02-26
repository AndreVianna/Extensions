using IConfigurationBuilder = DotNetToolbox.ConsoleApplication.Application.IConfigurationBuilder;

namespace DotNetToolbox.ConsoleApplication;

public class ApplicationBaseTests {
    [Fact]
    public void Create_WhenCreationFails_Throws() {
        // Arrange & Act
        var app = TestApplication.Create(b => b.ConfigureLogging(l => l.SetMinimumLevel(LogLevel.Information)));

        // Assert
        app.Should().BeOfType<TestApplication>();
    }

    [Fact]
    public void Run_WithClearScreen_Runs() {
        // Arrange
        var option = new TestOutput();
        var app = TestApplication.Create(["-cls"], b => b.SetOutputHandler(option));

        // Act
        var action = () => app.Run();

        // Assert
        action.Should().NotThrow();
    }

    [Fact]
    public void Create_CreatesTestApplication_WithCustomAttributes() {
        // Arrange & Act
        var assemblyDescriptor = Substitute.For<IAssemblyDescriptor>();
        assemblyDescriptor.Name.Returns("TestApp");
        assemblyDescriptor.Version.Returns(new Version(1, 0));
        assemblyDescriptor.GetCustomAttribute<AssemblyTitleAttribute>().Returns(new AssemblyTitleAttribute("My App"));
        assemblyDescriptor.GetCustomAttribute<AssemblyDescriptionAttribute>().Returns(new AssemblyDescriptionAttribute("Some description."));
        var app = TestApplication.Create(b => b.SetAssemblyInformation(assemblyDescriptor));

        // Assert
        app.Should().BeOfType<TestApplication>();
        app.Name.Should().Be("My App");
        app.AssemblyName.Should().Be("TestApp");
        app.Version.Should().Be("1.0");
        app.Help.Should().Be("Some description.");
    }

    [Fact]
    public void Create_CreatesTestApplication_WithAssemblyInfo() {
        // Arrange & Act
        var assemblyDescriptor = Substitute.For<IAssemblyDescriptor>();
        assemblyDescriptor.Name.Returns("TestApp");
        assemblyDescriptor.Version.Returns(new Version(1, 0));
        var app = TestApplication.Create(b => b.SetAssemblyInformation(assemblyDescriptor));

        // Assert
        app.Should().BeOfType<TestApplication>();
        app.Name.Should().Be("TestApp");
        app.AssemblyName.Should().Be("TestApp");
        app.Version.Should().Be("1.0");
    }

    [Fact]
    public void Create_AddEnvironmentVariables_CreatesTestApplication() {
        // Act
        var app = TestApplication.Create(b => b.AddEnvironmentVariables("MYAPP_"));

        // Assert
        app.Should().BeOfType<TestApplication>();
    }

    [Fact]
    public void Create_AddUserSecrets_CreatesTestApplication() {
        // Act
        var app = TestApplication.Create(b => b.AddUserSecrets<TestApplication>());

        // Assert
        app.Should().BeOfType<TestApplication>();
    }

    [Fact]
    public void Create_WithConfig_CreatesTestApplication() {
        // Arrange & Act
        var setConfigCalled = false;
        var configBuilderCalled = false;
        var app = TestApplication.Create(_ => setConfigCalled = true, _ => configBuilderCalled = true);

        // Assert
        app.Should().BeOfType<TestApplication>();
        setConfigCalled.Should().BeTrue();
        configBuilderCalled.Should().BeTrue();
    }

    [Fact]
    public void Create_SetEnvironment_CreatesTestApplication() {
        // Arrange & Act
        var app = TestApplication.Create(["--environment", "Development"]);

        // Assert
        app.Should().BeOfType<TestApplication>();
    }

    [Fact]
    public void Create_AddSettings_CreatesTestApplication() {
        // Arrange
        var fileProvider = new TestFileProvider();

        // Act
        var app = TestApplication.Create(b => b.AddAppSettings(fileProvider));

        // Assert
        app.Should().BeOfType<TestApplication>();
    }

    [Fact]
    public void Create_AddSettings_WithEnvironmentSet_CreatesTestApplication() {
        // Arrange
        var fileProvider = new TestFileProvider();

        // Act
        var app = TestApplication.Create(["-env", "Development"], b => b.AddAppSettings(fileProvider));

        // Assert
        app.Should().BeOfType<TestApplication>();
    }

    [Fact]
    public void Create_SetLogging_CreatesTestApplication() {
        // Arrange & Act
        var app = TestApplication.Create(b
                                   => b.ConfigureLogging(l => l.SetMinimumLevel(LogLevel.Debug)));

        // Assert
        app.Should().BeOfType<TestApplication>();
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider) {
            Help = "This is a test Application.",
        };

        var expectedToString = $"TestApplication: {app.Name} v{app.Version} => {app.Help}";

        // Act
        var actualToString = app.ToString();

        // Assert
        actualToString.Should().Be(expectedToString);
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
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        var result = () => app.AddCommand("command", action);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddCommand_AndSubCommand_AddsCommand() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        var command = (Command)app.AddCommand("command", (Action)(() => throw new()));

        // Act
        var subCommand = command.AddCommand("sub-command", (Action)(() => throw new()));

        // Assert
        subCommand.Path.Should().Be("TestApp command sub-command");
    }

    [Fact]
    public async Task AddCommand_WithException_AddsCommandThatThrows() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        var command = (Command)app.AddCommand("command", (Action)(() => throw new()));

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
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        var subject = app.AddCommand("command", action);

        // Assert
        app.Children.Should().ContainSingle(x => x.Name == "command");
        var command = subject.Should().BeOfType<Command>().Subject;
        command.Aliases.Should().BeEmpty();
        var result = await command.Execute([]);
        result.Should().Be(Result.Success());
    }

    [Theory]
    [InlineData(null, "c")]
    [InlineData("", "c")]
    [InlineData("c", "c")]
    [InlineData("2cmd", "c")]
    [InlineData("-c$h", "c")]
    [InlineData("c$h", "c")]
    [InlineData("c-h$", "c")]
    [InlineData("cmd", null)]
    [InlineData("cmd", "")]
    [InlineData("cmd", "=")]
    [InlineData("cmd", "-")]
    [InlineData("cmd", "2-")]
    public void AddCommand_WithInvalidNameOrAlias_Throws(string? name, string? alias) {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        var action = () => app.AddCommand(name!, [alias!], () => { });

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [ClassData(typeof(CommandDelegates))]
    public void AddCommand_WithAlias_AddsCommand(Delegate action) {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        app.AddCommand("command", "c", action);

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "command").Subject;
        child.Aliases.Should().BeEquivalentTo("c");
    }

    [Fact]
    public async Task AddCommand_OfType_AddsCommandOfType() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        app.AddCommand<TestCommand>();

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "Command").Subject;
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
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        var node = new TestCommand(app);

        // Act
        app.AddCommand(node);

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "Command").Subject;
        child.Should().BeOfType<TestCommand>();
    }

    [Fact]
    public void AddOption_AddsOption() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        app.AddOption("option");

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "option").Subject;
        var option = child.Should().BeOfType<Option>().Subject;
        option.Aliases.Should().BeEmpty();
    }

    [Fact]
    public void AddOption_WithAlias_AddsOption() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        app.AddOption("option", "o");

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "option").Subject;
        var option = child.Should().BeOfType<Option>().Subject;
        option.Aliases.Should().BeEquivalentTo("o");
    }

    [Fact]
    public void AddOption_OfType_AddsOptionOfType() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        app.AddOption<TestOption>();

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "MultipleChoiceOption").Subject;
        var option = child.Should().BeOfType<TestOption>().Subject;
        option.Aliases.Should().BeEquivalentTo("o");
    }

    [Fact]
    public void AddOption_WithOption_AddsOption() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        var node = new TestOption(app);

        // Act
        app.AddOption(node);

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "MultipleChoiceOption").Subject;
        child.Should().BeOfType<TestOption>();
    }

    [Fact]
    public void AddParameter_AddsParameter() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        const string parameterName = "param1";

        // Act
        app.AddParameter(parameterName);

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == parameterName).Subject;
        var parameter = child.Should().BeOfType<Parameter>().Subject;
        parameter.Order.Should().Be(0);
        parameter.IsRequired.Should().BeTrue();
        parameter.IsSet.Should().BeFalse();
    }

    [Fact]
    public void AddParameter_WithDefaultValue_AddsParameter() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        const string parameterName = "param1";
        const string defaultValue = "defaultValue";

        // Act
        app.AddParameter(parameterName, defaultValue);

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == parameterName).Subject;
        var parameter = child.Should().BeOfType<Parameter>().Subject;
        parameter.Order.Should().Be(0);
        parameter.IsRequired.Should().BeFalse();
        parameter.IsSet.Should().BeFalse();
    }

    [Fact]
    public void AddParameter_OfType_AddsParameterOfType() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        app.AddParameter<TestParameter>();

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "Age").Subject;
        var parameter = child.Should().BeOfType<TestParameter>().Subject;
        parameter.Aliases.Should().BeEmpty();
        parameter.Order.Should().Be(0);
    }

    [Fact]
    public void AddParameter_WithParameter_AddsParameter() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        var node = new TestParameter(app);

        // Act
        app.AddParameter(node);

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "Age").Subject;
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
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        var result = () => app.AddFlag("flag", action);

        // Assert
        result.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task AddFlag_WithException_AddsFlagThatThrows() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        var flag = app.AddFlag("flag", (Action)(() => throw new()));
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
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        var context = new Map();

        // Act
        var subject = app.AddFlag("flag", action);

        // Assert
        app.Children.Should().ContainSingle(x => x.Name == "flag");
        var flag = subject.Should().BeOfType<Flag>().Subject;
        flag.Aliases.Should().BeEmpty();
        var result = await subject.Read(context);
        result.Should().Be(Result.Success());
    }

    [Theory]
    [ClassData(typeof(FlagDelegates))]
    public void AddFlag_WithAlias_AddsFlag(Delegate action) {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        app.AddFlag("flag", "c", action);

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "flag").Subject;
        child.Aliases.Should().BeEquivalentTo("c");
    }

    [Fact]
    public void AddFlag_OfType_AddsFlagOfType() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);

        // Act
        app.AddFlag<TestFlag>();

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "Flag").Subject;
        var flag = child.Should().BeOfType<TestFlag>().Subject;
        flag.Aliases.Should().BeEquivalentTo("f");
    }

    [Fact]
    public void AddFlag_WithFlag_AddsFlag() {
        // Arrange
        var serviceProvider = CreateFakeServiceProvider();
        var app = new TestApplication([], serviceProvider);
        var node = new TestFlag(app);

        // Act
        app.AddFlag(node);

        // Assert
        var child = app.Children.Should().ContainSingle(x => x.Name == "Flag").Subject;
        child.Should().BeOfType<TestFlag>();
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestCommand
        : Command<TestCommand> {
        public TestCommand(IHasChildren app)
            : base(app, "Command", c => c.Aliases = ["c"]) {
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
    private IServiceCollection CreateFakeServiceProvider() {
        var output = new TestOutput();
        var input = new TestInput(output);
        var environment = Substitute.For<IApplicationEnvironment>();
        var assembly = Substitute.For<IAssemblyDescriptor>();
        environment.Assembly.Returns(assembly);
        assembly.Name.Returns("TestApp");
        assembly.Version.Returns(new Version(1, 0));
        var services = new ServiceCollection();
        services.AddSingleton(Substitute.For<IConfigurationRoot>());
        services.AddKeyedSingleton("TestApp", _assemblyDescriptor);
        services.AddKeyedSingleton("TestApp", Substitute.For<IDateTimeProvider>());
        services.AddKeyedSingleton("TestApp", Substitute.For<IGuidProvider>());
        services.AddKeyedSingleton("TestApp", Substitute.For<IFileSystemAccessor>());
        services.AddKeyedSingleton<IOutput>("TestApp", output);
        services.AddKeyedSingleton<IInput>("TestApp", input);
        services.AddSingleton(environment);
        _assemblyDescriptor.Name.Returns("TestApp");
        _assemblyDescriptor.Version.Returns(new Version(1, 0));
        services.AddSingleton(Substitute.For<ILoggerFactory>());
        return services;
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestApplication(string[] args, IServiceCollection services)
        : ApplicationBase<TestApplication, TestApplicationBuilder, ApplicationSettings>(args, services) {
        internal override Task Run(CancellationToken ct = default)
            => Result.Task.FromResult(Success());
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestApplicationBuilder(string[] args, Action<IConfigurationBuilder>? configure = null)
        : ApplicationBuilder<TestApplication, TestApplicationBuilder, ApplicationSettings>(args, configure);
}
