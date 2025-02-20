namespace DotNetToolbox.ConsoleApplication.Application;

public abstract class ApplicationBase<TApplication, TBuilder, TSettings>
    : ApplicationBase<TSettings>,
      IApplication<TApplication, TBuilder, TSettings>
    where TApplication : ApplicationBase<TApplication, TBuilder, TSettings>
    where TBuilder : ApplicationBuilder<TApplication, TBuilder, TSettings>
    where TSettings : ApplicationSettings, new() {
    protected ApplicationBase(string[] args, IServiceCollection services)
        : base(args, services) {
        services.AddSingleton((TApplication)this);
        services.AddSingleton(_ => new Lazy<TApplication>(() => (TApplication)this));
    }

    public static TBuilder CreateBuilder(Action<IConfigurationBuilder>? setConfiguration = null)
        => CreateBuilder([], setConfiguration);
    public static TBuilder CreateBuilder(string[] args, Action<IConfigurationBuilder>? setConfiguration = null) => InstanceFactory.Create<TBuilder>(args, setConfiguration ?? DefaultAction);

    private static void DefaultAction(IConfigurationBuilder _) { }

    public static TApplication Create(string[] args, Action<IConfigurationBuilder> setConfiguration, Action<TBuilder> configureBuilder) {
        var builder = CreateBuilder(args, setConfiguration);
        configureBuilder(builder);
        return builder.Build();
    }

    public static TApplication Create(Action<IConfigurationBuilder> setConfiguration, Action<TBuilder> configureBuilder)
        => Create([], setConfiguration, configureBuilder);

    public static TApplication Create(string[] args, Action<IConfigurationBuilder> setConfiguration)
        => Create(args, setConfiguration, null!);

    public static TApplication Create(Action<IConfigurationBuilder> setConfiguration)
        => Create([], setConfiguration, null!);

    public static TApplication Create(string[] args, Action<TBuilder> configureBuilder)
        => Create(args, null!, configureBuilder);

    public static TApplication Create(Action<TBuilder> configureBuilder)
        => Create([], null!, configureBuilder);

    public static TApplication Create(string[] args)
        => Create(args, null!, null!);

    public static TApplication Create()
        => Create([], null!, null!);

    public override string ToString()
        => $"{GetType().Name}: {Name} v{Version} => {Help}";

    public int Run() => RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    public async Task<int> RunAsync() {
        try {
            IsRunning = true;
            var taskRun = new CancellationTokenSource();
            if (Context.TryGetValue("ClearScreenOnStart", out var clearScreen) && clearScreen is true)
                Output.ClearScreen();
            if (await TryParseArguments(taskRun.Token).ConfigureAwait(false))
                return IApplication.DefaultErrorCode;
            if (!IsRunning) return ExitCode;
            await Run(taskRun.Token).ConfigureAwait(false);
            return ExitCode;
        }
        catch (ConsoleException ex) {
            HandleException(ex);
            Output.WriteError(ex);
            return ex.ExitCode;
        }
        catch (Exception ex) {
            HandleException(ex);
            Output.WriteError(ex);
            return IApplication.DefaultErrorCode;
        }
    }

    protected void ProcessResult(IValidationResult result) {
        if (!result.HasErrors) return;
        var errors = new ResultErrors(result.Errors);
        Output.WriteLine(errors.ToText());
    }

    protected async Task<bool> TryParseArguments(CancellationToken ct) {
        var result = await ArgumentsParser.Parse(this, Arguments, ct).ConfigureAwait(false);
        ProcessResult(result);
        return result.HasErrors;
    }

    protected virtual bool HandleException<TException>(TException ex)
        where TException : Exception => false;

    protected Task<IValidationResult> ProcessCommand(string command, CancellationToken ct)
        => ProcessCommand([command], ct);

    protected Task<IValidationResult> ProcessCommand(string command, string[] args, CancellationToken ct)
        => ProcessCommand([command, .. args], ct);

    protected virtual async Task<IValidationResult> ProcessCommand(string[] input, CancellationToken ct) {
        if (input.Length == 0) return Success();
        var command = FindCommand((this as IHasChildren).Commands, input[0]);
        if (command is null) return Failure($"Command '{input[0]}' not found. For a list of available commands use 'help'.");
        var arguments = input.Skip(1).ToArray();
        return await command.Execute(arguments, ct);
    }

    private static ICommand? FindCommand(IEnumerable<ICommand> commands, string token)
        => token.StartsWith('"') || token.StartsWith('-')
               ? null
               : commands.FirstOrDefault(c => c.Name.Equals(token, StringComparison.OrdinalIgnoreCase)
                                           || c.Aliases.Contains(token));
}

public abstract partial class ApplicationBase<TSettings>
    : IApplication<TSettings>
    where TSettings : ApplicationSettings, new() {
    private readonly AsyncServiceScope _servicesScope;
    private string _description = string.Empty;

    protected ApplicationBase(string[] args, IServiceCollection services) {
        Arguments = args;
        services.AddSingleton<IApplication>(this);
        services.AddSingleton<Lazy<IApplication>>(_ => new(() => this));
        var provider = services.BuildServiceProvider();
        _servicesScope = provider.CreateAsyncScope();
        Services = _servicesScope.ServiceProvider;
        Logger = Services.GetRequiredService<ILoggerFactory>().CreateLogger(GetType().Name);
        Environment = Services.GetRequiredService<IApplicationEnvironment>();
        Configuration = Services.GetRequiredService<IConfigurationRoot>();

        AssemblyName = Environment.Assembly.Name;
        var ata = Environment.Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        Name = ata ?? AssemblyName;
        Settings = new();
        Configuration.GetSection(nameof(Settings)).Bind(Settings);

        Version = Environment.Assembly.Version.ToString();
        DisplayVersion = Environment.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? Version;
        var plusIndex = DisplayVersion.IndexOf('+');
        if (plusIndex > 0) DisplayVersion = DisplayVersion[..plusIndex];
        DisplayVersion = VersionPatch.Replace(DisplayVersion, "-$1");

        var ada = Environment.Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        Description = ada ?? string.Empty;

        AddFlag<HelpFlag>();
        AddFlag<ClearScreenFlag>();
        AddFlag<VersionFlag>();
        IsRunning = false;
    }

    protected virtual ValueTask Dispose()
        => _servicesScope.DisposeAsync();

    public async ValueTask DisposeAsync() {
        await Dispose();
        GC.SuppressFinalize(this);
    }

    public int ExitCode { get; protected set; } = IApplication.DefaultExitCode;

    public string AssemblyName { get; }
    public string Name { get; }
    public string Version { get; }
    public string DisplayVersion { get; }
    public string FullName => $"{Name} v{DisplayVersion}";
    public string Description {
        get => _description;
        set {
            _description = value;
            if (string.IsNullOrWhiteSpace(Help)) Help = _description;
        }
    }
    public string Help { get; set; } = string.Empty;
    public TSettings Settings { get; init; }

    public ILogger Logger { get; }

    public IServiceProvider Services { get; }
    public IConfigurationRoot Configuration { get; }
    public IApplicationEnvironment Environment { get; }
    public IOutput Output => Environment.OperatingSystem.Output;
    public IInput Input => Environment.OperatingSystem.Input;
    protected IFileSystemAccessor FileSystem => Environment.OperatingSystem.FileSystem;
    protected IAssemblyDescriptor Assembly => Environment.Assembly;
    protected IDateTimeProvider DateTime => Environment.OperatingSystem.DateTime;
    protected IGuidProvider Guid => Environment.OperatingSystem.Guid;
    public IMap Context { get; } = new Map();

    public ICollection<INode> Children { get; } = [];
    public IParameter[] Parameters => [.. Children.OfType<IParameter>().OrderBy(i => i.Order)];
    public IArgument[] Options => [.. Children.OfType<IArgument>().OrderBy(i => i.Name)];
    public ICommand[] Commands => [.. Children.OfType<ICommand>().Except(Options.Cast<INode>()).Cast<ICommand>().OrderBy(i => i.Name)];

    protected virtual Task<IValidationResult> OnStart(CancellationToken ct = default)
        => Task.FromResult(Success());
    protected virtual IValidationResult OnExit() => Success();

    public virtual void Exit(int code = IApplication.DefaultExitCode) {
        OnExit();
        ExitCode = code;
        IsRunning = false;
    }

    protected string[] Arguments { get; }
    protected bool IsRunning { get; set; }

    IApplication INode.Application => this;
    string[] INode.Aliases => [];

    internal abstract Task Run(CancellationToken ct = default);

    public ICommand AddCommand(string name, Delegate action)
        => AddCommand(name, aliases: [], action);
    public ICommand AddCommand(string name, string alias, Delegate action)
        => AddCommand(name, [alias], action);
    public ICommand AddCommand(string name, string[] aliases, Delegate action)
        => NodeFactory.Create<Command>(this, name, (Action<Parameter>)(n => n.Aliases = aliases), action);
    public ICommand AddCommand<TChildCommand>()
        where TChildCommand : Command<TChildCommand>, ICommand
        => NodeFactory.Create<TChildCommand>(this);
    public void AddCommand(ICommand command) => Children.Add(command);

    public IFlag AddFlag(string name, Delegate? action = null)
        => AddFlag(name, aliases: [], action);
    public IFlag AddFlag(string name, string alias, Delegate? action = null)
        => AddFlag(name, [alias], action);
    public IFlag AddFlag(string name, string[] aliases, Delegate? action = null)
        => NodeFactory.Create<Flag>(this, name, (Action<Parameter>)(n => n.Aliases = aliases), action);
    public IFlag AddFlag<TFlag>()
        where TFlag : Flag<TFlag>, IFlag
        => NodeFactory.Create<TFlag>(this);
    public void AddFlag(IFlag flag) => Children.Add(flag);

    public IOption AddOption(string name)
        => AddOption(name, aliases: []);
    public IOption AddOption(string name, string alias)
        => AddOption(name, [alias]);
    public IOption AddOption(string name, string[] aliases)
        => NodeFactory.Create<Option>(this, name, (Action<Parameter>)(n => n.Aliases = aliases));
    public IOption AddOption<TOption>()
        where TOption : Option<TOption>, IOption
        => NodeFactory.Create<TOption>(this);
    public void AddOption(IOption option) => Children.Add(option);

    public IParameter AddParameter(string name)
        => NodeFactory.Create<Parameter>(this, name);
    public IParameter AddParameter(string name, string defaultValue)
        => NodeFactory.Create<Parameter>(this, name, (Action<Parameter>)(n => n.DefaultValue = IsNotNull(defaultValue)));
    public IParameter AddParameter<TParameter>()
        where TParameter : Parameter<TParameter>, IParameter
        => NodeFactory.Create<TParameter>(this);
    public void AddParameter(IParameter parameter) => Children.Add(parameter);

    [GeneratedRegex(@"\.([a-z]\S*)$", IgnoreCase | Compiled | Singleline)]
    private static partial Regex VersionPatch { get; }
}
