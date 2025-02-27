using Shell = DotNetToolbox.ConsoleApplication.ShellApplication;

namespace DotNetToolbox.ConsoleApplication;

public sealed class ShellApplicationTests {
    [Fact]
    public void Create_CreatesShellApplication() {
        // Arrange & Act
        var app = Shell.Create();

        // Assert
        app.Should().BeOfType<Shell>();
        app.Name.Should().Be("testhost");
        app.Version.Should().Be("15.0.0.0");
        app.Environment.Name.Should().Be("");
        app.AssemblyName.Should().Be("testhost");
        app.Children.Should().HaveCount(6);
        app.Context.Should().BeEmpty();
        app.Logger.Should().NotBeNull();
    }

    [Fact]
    public void ReplaceInput_ExecutesUntilExit() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output);

        // Act
        var app = Shell.Create(b => b.SetInputHandler(input));

        // Assert
        app.Should().BeOfType<Shell>();
    }

    [Fact]
    public void Run_ExecutesUntilExit() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "", "--exit", "\"exit\"", "exit");
        const string expectedOutput =
            """
            testhost v15.0.0.0
            >
            > --exit
            > "exit"
            > exit

            """;
        var fileSystem = new TestFileSystemAccessor();
        var guidProvider = new TestGuidProvider();
        var dateTimeProvider = new TestDateTimeProvider();
        var app = Shell.Create(b => {
            b.SetDateTimeProvider(dateTimeProvider);
            b.SetGuidProvider(guidProvider);
            b.SetFileSystem(fileSystem);
            b.SetOutputHandler(output);
            b.SetInputHandler(input);
        });

        // Act
        app.Run();

        // Assert
        app.Should().BeOfType<Shell>();
        output.ToString().Should().Be(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithSetOptionsAndProvidersReplaced_ExecutesUntilExit() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "exit");
        const string expectedOutput =
            """
            testhost v15.0.0.0
            > exit

            """;
        var fileSystem = new TestFileSystemAccessor();
        var guidProvider = new TestGuidProvider();
        var dateTimeProvider = new TestDateTimeProvider();
        await using var app = Shell.Create(b => {
            b.SetDateTimeProvider(dateTimeProvider);
            b.SetGuidProvider(guidProvider);
            b.SetFileSystem(fileSystem);
            b.SetOutputHandler(output);
            b.SetInputHandler(input);
        });

        // Act
        await app.RunAsync();

        // Assert
        app.Should().BeOfType<Shell>();
        output.ToString().Should().Be(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithHelp_ExecutesUntilExit() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "help", "exit");
        var assemblyDescriptor = Substitute.For<IAssemblyDescriptor>();
        assemblyDescriptor.Name.Returns("tsa");
        assemblyDescriptor.Version.Returns(new Version(1, 0));
        assemblyDescriptor.GetCustomAttribute<AssemblyTitleAttribute>().Returns(new AssemblyTitleAttribute("Test Shell Application"));
        assemblyDescriptor.GetCustomAttribute<AssemblyDescriptionAttribute>().Returns(new AssemblyDescriptionAttribute("This is a test application."));
        const string expectedOutput =
            """
            Test Shell Application v1.0
            > help
            Test Shell Application v1.0
            This is a test application.

            Usage:
                tsa [Options] [Commands]
                tsa [Options] [<Timeout>]

            Options:
                --clear-screen, -cls      Clear the screen.
                --help, -h, -?            Display this help information.
                --version                 Display the application's version.

            Parameters:
                Timeout

            Commands:
                ClearScreen, cls          Clear the screen.
                Exit                      Exit the application.
                Help, ?                   Display this help information.

            > exit

            """;
        await using var app = Shell.Create(b => {
            b.SetAssemblyInformation(assemblyDescriptor);
            b.SetOutputHandler(output);
            b.SetInputHandler(input);
        });
        app.AddParameter("Timeout", "5000");

        // Act
        await app.RunAsync();

        // Assert
        app.Should().BeOfType<Shell>();
        output.ToString().Should().Be(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithHelpAlias_ExecutesUntilExit() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "?", "exit");
        var assemblyDescriptor = Substitute.For<IAssemblyDescriptor>();
        assemblyDescriptor.Name.Returns("tsa");
        assemblyDescriptor.Version.Returns(new Version(1, 0));
        assemblyDescriptor.GetCustomAttribute<AssemblyTitleAttribute>().Returns(new AssemblyTitleAttribute("Test Shell Application"));
        assemblyDescriptor.GetCustomAttribute<AssemblyDescriptionAttribute>().Returns(new AssemblyDescriptionAttribute("This is a test application."));
        const string expectedOutput =
            """
            Test Shell Application v1.0
            > ?
            Test Shell Application v1.0
            This is a test application.

            Usage:
                tsa [Options] [Commands]
                tsa [Options] [<Timeout>]

            Options:
                --clear-screen, -cls      Clear the screen.
                --help, -h, -?            Display this help information.
                --version                 Display the application's version.

            Parameters:
                Timeout

            Commands:
                ClearScreen, cls          Clear the screen.
                Exit                      Exit the application.
                Help, ?                   Display this help information.

            > exit

            """;
        await using var app = Shell.Create(b => {
            b.SetAssemblyInformation(assemblyDescriptor);
            b.SetOutputHandler(output);
            b.SetInputHandler(input);
        });
        app.AddParameter("Timeout", "5000");

        // Act
        await app.RunAsync();

        // Assert
        app.Should().BeOfType<Shell>();
        output.ToString().Should().Be(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithHelpCommand_ExecutesUntilExit() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "help exit", "exit");
        var assemblyDescriptor = Substitute.For<IAssemblyDescriptor>();
        assemblyDescriptor.Name.Returns("tsa");
        assemblyDescriptor.Version.Returns(new Version(1, 0));
        assemblyDescriptor.GetCustomAttribute<AssemblyTitleAttribute>().Returns(new AssemblyTitleAttribute("Test Shell Application"));
        assemblyDescriptor.GetCustomAttribute<AssemblyDescriptionAttribute>().Returns(new AssemblyDescriptionAttribute("This is a test application."));
        const string expectedOutput =
            """
            Test Shell Application v1.0
            > help exit
            Exit the application.

            Usage:
                Exit

            > exit

            """;
        await using var app = Shell.Create(b => {
            b.SetAssemblyInformation(assemblyDescriptor);
            b.SetOutputHandler(output);
            b.SetInputHandler(input);
        });
        app.AddParameter("Timeout", "5000");

        // Act
        await app.RunAsync();

        // Assert
        app.Should().BeOfType<Shell>();
        output.ToString().Should().Be(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithHelpOption_ShowsHelpAndExits() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output);
        var assemblyDescriptor = Substitute.For<IAssemblyDescriptor>();
        assemblyDescriptor.Name.Returns("tsa");
        assemblyDescriptor.Version.Returns(new Version(1, 0));
        assemblyDescriptor.GetCustomAttribute<AssemblyTitleAttribute>().Returns(new AssemblyTitleAttribute("Test Shell Application"));
        assemblyDescriptor.GetCustomAttribute<AssemblyDescriptionAttribute>().Returns(new AssemblyDescriptionAttribute("This is a test application."));
        const string expectedOutput =
            """
            Test Shell Application v1.0
            This is a test application.

            Usage:
                tsa [Options] [Commands]
                tsa [Options] [<Timeout>]

            Options:
                --clear-screen, -cls      Clear the screen.
                --help, -h, -?            Display this help information.
                --version                 Display the application's version.

            Parameters:
                Timeout

            Commands:
                ClearScreen, cls          Clear the screen.
                Exit                      Exit the application.
                Help, ?                   Display this help information.


            """;
        await using var app = Shell.Create(["--help"], b => {
            b.SetAssemblyInformation(assemblyDescriptor);
            b.SetOutputHandler(output);
            b.SetInputHandler(input);
        });
        app.AddParameter("Timeout", "5000");

        // Act
        await app.RunAsync();

        // Assert
        app.Should().BeOfType<Shell>();
        output.ToString().Should().Be(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithHelpCommand_ShowsHelp() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "help ClearScreen", "exit");
        var assemblyDescriptor = Substitute.For<IAssemblyDescriptor>();
        assemblyDescriptor.Name.Returns("tsa");
        assemblyDescriptor.Version.Returns(new Version(1, 0));
        assemblyDescriptor.GetCustomAttribute<AssemblyTitleAttribute>().Returns(new AssemblyTitleAttribute("Test Shell Application"));
        assemblyDescriptor.GetCustomAttribute<AssemblyDescriptionAttribute>().Returns(new AssemblyDescriptionAttribute("This is a test application."));
        const string expectedOutput =
            """
            Test Shell Application v1.0
            > help ClearScreen
            Clear the screen.

            Usage:
                ClearScreen

            Aliases: cls

            > exit

            """;
        var app = Shell.Create(b => {
            b.SetAssemblyInformation(assemblyDescriptor);
            b.SetOutputHandler(output);
            b.SetInputHandler(input);
        });
        // Act
        await app.RunAsync();

        // Assert
        app.Should().BeOfType<Shell>();
        output.ToString().Should().Be(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithExceptionDuringExecution_ReturnsResultWithException() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "crash");

        static Result CommandAction() => throw new("Some error.");
        const string expectedOutput =
            """
            testhost v15.0.0.0
            > crash
            Exception: Some error.
                Stack Trace:
                       at DotNetToolbox.ConsoleApplication.ShellApplicationTests.*
                       at DotNetToolbox.ConsoleApplication.Utilities.NodeFactory*
                       at DotNetToolbox.ConsoleApplication.Nodes.Command`1*
                       at DotNetToolbox.ConsoleApplication.Nodes.Command`1*
                       at DotNetToolbox.ConsoleApplication.Application.ApplicationBase`2.*
                       at DotNetToolbox.ConsoleApplication.ShellApplication`2.*
                       at DotNetToolbox.ConsoleApplication.ShellApplication`2.*
                       at DotNetToolbox.ConsoleApplication.Application.ApplicationBase`2.*


            """;
        var app = Shell.Create(b => {
            b.SetInputHandler(input);
            b.SetOutputHandler(output);
        });
        app.AddCommand("Crash", CommandAction);

        // Act
        var actualResult = await app.RunAsync();

        // Assert
        actualResult.Should().Be(IApplication.DefaultErrorCode);
        output.ToString().Should().Match(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithConsoleExceptionDuringExecution_ReturnsResultWithException() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "crash");
        const int expectedErrorCode = 13;

        static Result CommandAction()
            => throw new ConsoleException(expectedErrorCode, "Some error.");
        const string expectedOutput =
            """
            testhost v15.0.0.0
            > crash
            ConsoleException: Some error.
                Stack Trace:
                       at DotNetToolbox.ConsoleApplication.ShellApplicationTests*
                       at DotNetToolbox.ConsoleApplication.Utilities.NodeFactory*
                       at DotNetToolbox.ConsoleApplication.Nodes.Command`1*
                       at DotNetToolbox.ConsoleApplication.Nodes.Command`1*
                       at DotNetToolbox.ConsoleApplication.Application.ApplicationBase`2.ProcessCommand(String[] input, CancellationToken ct)*
                       at DotNetToolbox.ConsoleApplication.ShellApplication`2.ProcessInput(String input, CancellationToken ct)*
                       at DotNetToolbox.ConsoleApplication.ShellApplication`2.Run(CancellationToken ct)*
                       at DotNetToolbox.ConsoleApplication.Application.ApplicationBase`2.RunAsync()*


            """;
        var app = Shell.Create(b => {
            b.SetInputHandler(input);
            b.SetOutputHandler(output);
        });
        app.AddCommand("Crash", CommandAction);

        // Act
        var actualResult = await app.RunAsync();

        // Assert
        actualResult.Should().Be(expectedErrorCode);
        output.ToString().Should().Match(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithConsoleExceptionDuringExecution_AndInnerException_ReturnsResultWithException() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "crash");
        const int expectedErrorCode = 13;

        static Result CommandAction() {
            try {
                throw new InvalidOperationException("Some error.");
            }
            catch (Exception ex) {
                throw new ConsoleException(expectedErrorCode, "Some error.", ex);
            }
        }
        const string expectedOutput =
            """
            testhost v15.0.0.0
            > crash
            ConsoleException: Some error.
                Stack Trace:
                       at DotNetToolbox.ConsoleApplication.ShellApplicationTests*
                       at DotNetToolbox.ConsoleApplication.Utilities.NodeFactory*
                       at DotNetToolbox.ConsoleApplication.Nodes.Command`1.ExecuteAsync*
                       at DotNetToolbox.ConsoleApplication.Nodes.Command`1.DotNetToolbox.ConsoleApplication.Nodes.ICommand.Set*
                       at DotNetToolbox.ConsoleApplication.Application.ApplicationBase`2.ProcessCommand*
                       at DotNetToolbox.ConsoleApplication.ShellApplication`2.ProcessInput(String input, CancellationToken ct)*
                       at DotNetToolbox.ConsoleApplication.ShellApplication`2.Run(CancellationToken ct)*
                       at DotNetToolbox.ConsoleApplication.Application.ApplicationBase`2.RunAsync()*
                Inner Exception => InvalidOperationException: Some error.
                    Stack Trace:
                           at DotNetToolbox.ConsoleApplication.ShellApplicationTests*


            """;
        var app = Shell.Create(b => {
            b.SetInputHandler(input);
            b.SetOutputHandler(output);
        });
        app.AddCommand("Crash", CommandAction);

        // Act
        var actualResult = await app.RunAsync();

        // Assert
        actualResult.Should().Be(expectedErrorCode);
        output.ToString().Should().Match(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithErrorDuringExecution_ReturnsResultWithErrors() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "crash", "exit");

        static Result CommandAction() => Result.Failure("Some error.");
        const string expectedOutput =
            """
            testhost v15.0.0.0
            > crash
            Validation error: Some error.

            > exit

            """;
        var app = Shell.Create(b => {
            b.SetInputHandler(input);
            b.SetOutputHandler(output);
        });
        app.AddCommand("Crash", CommandAction);

        // Act
        var actualResult = await app.RunAsync();

        // Assert
        actualResult.Should().Be(IApplication.DefaultExitCode);
        output.ToString().Should().Be(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_WithErrorDuringArgumentRead_ReturnsResultWithErrors() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "exit");
        const string expectedOutput =
            """
            Validation error: Unknown argument '--invalid'. For a list of available arguments use '--help'.


            """;
        var app = Shell.Create(["--invalid"], b => {
            b.SetInputHandler(input);
            b.SetOutputHandler(output);
        });

        // Act
        var actualResult = await app.RunAsync();

        // Assert
        actualResult.Should().Be(IApplication.DefaultErrorCode);
        output.ToString().Should().Be(expectedOutput);
    }

    [Fact]
    public async Task RunAsync_InvalidCommand_ReturnsResultWithErrors() {
        // Arrange
        var output = new TestOutput();
        var input = new TestInput(output, "invalid", "exit");
        const string expectedOutput =
            """
            testhost v15.0.0.0
            > invalid
            > exit

            """;
        var app = Shell.Create(b => {
            b.SetInputHandler(input);
            b.SetOutputHandler(output);
        });

        // Act
        var actualResult = await app.RunAsync();

        // Assert
        actualResult.Should().Be(IApplication.DefaultExitCode);
        output.ToString().Should().Be(expectedOutput);
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestShellApp(string[] args, IServiceCollection services)
        : ShellApplication<TestShellApp, ApplicationSettings>(args, services) {
        protected override Task<Result> OnStart(CancellationToken ct = default)
            => Task.FromResult(Result.Failure("Some error."));
    }

    [Fact]
    public async Task RunAsync_WithInvalidAction_ExecutesApp() {
        var output = new TestOutput();
        var input = new TestInput(output, "exit");
        var app = TestShellApp.Create(b => {
            b.SetInputHandler(input);
            b.SetOutputHandler(output);
        });

        // Act
        var actualResult = await app.RunAsync();

        // Assert
        actualResult.Should().Be(1);
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestFaultyShellApp(string[] args, IServiceCollection services)
        : ShellApplication<TestFaultyShellApp, ApplicationSettings>(args, services) {
        protected override Task<Result> OnStart(CancellationToken ct = default)
            => throw new ConsoleException(13);
    }

    [Fact]
    public async Task RunAsync_WithFaultyAction_ExecutesApp() {
        var output = new TestOutput();
        var input = new TestInput(output, "exit");
        var app = TestFaultyShellApp.Create(b => {
            b.SetInputHandler(input);
            b.SetOutputHandler(output);
        });

        // Act
        var actualResult = await app.RunAsync();

        // Assert
        actualResult.Should().Be(13);
    }

    [Fact]
    public async Task RunAsync_WithArguments_ExecutesApp() {
        var output = new TestOutput();
        var input = new TestInput(output, "exit");
        var app = Shell.Create(["MultipleChoiceOption", "o", "20"], b => {
            b.SetInputHandler(input);
            b.SetOutputHandler(output);
        });
        app.AddOption<TestOption>();
        app.AddFlag<TestFlag>();
        app.AddParameter<TestParameter>();

        // Act
        var actualResult = await app.RunAsync();

        // Assert
        actualResult.Should().Be(0);
    }

    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestOption(IHasChildren app) : Option<TestOption>(app, "MultipleChoiceOption", n => n.Aliases = ["o"]);
    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestParameter(IHasChildren app) : Parameter<TestParameter>(app, "Age", n => n.DefaultValue = "18");
    // ReSharper disable once ClassNeverInstantiated.Local - Used for tests.
    private sealed class TestFlag(IHasChildren app) : Flag<TestFlag>(app, "Flag", n => n.Aliases = ["f"]);
}
