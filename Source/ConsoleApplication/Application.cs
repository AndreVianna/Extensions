﻿namespace DotNetToolbox.ConsoleApplication;

public abstract class Application<TApplication, TBuilder, TOptions>
    : IApplication<TApplication, TBuilder, TOptions>
    where TApplication : Application<TApplication, TBuilder, TOptions>
    where TBuilder : ApplicationBuilder<TApplication, TBuilder, TOptions>
    where TOptions : ApplicationOptions<TOptions>, new() {
    private bool _isDisposed;

    internal Application(string[] args, string? environment, IServiceProvider serviceProvider) {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var options = serviceProvider.GetService<IOptions<TOptions>>();
        Options = options?.Value ?? new TOptions();
        Environment = environment ?? Options.Environment ?? string.Empty;
        Name = Options.Name ?? typeof(TApplication).Assembly.GetName().Name!;
        Version = Options.Version ?? typeof(TApplication).Assembly.GetName().Version!.ToString();
        Description = Options.Description;
        ServiceProvider = serviceProvider;
        Configuration = configuration;
        Arguments = args;

        Output = serviceProvider.GetRequiredService<Output>();
        Input = serviceProvider.GetRequiredService<Input>();
        DateTime = serviceProvider.GetRequiredService<DateTimeProvider>();
        Guid = serviceProvider.GetRequiredService<GuidProvider>();
        FileSystem = serviceProvider.GetRequiredService<FileSystem>();

        var loggerFactory = ServiceProvider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
        Logger = loggerFactory.CreateLogger<TApplication>();
    }

    protected virtual ValueTask Dispose()
        => ValueTask.CompletedTask;

    public string Environment { get; }
    public string Name { get; }
    public string Version { get; }
    public string? Description { get; }
    public string[] Arguments { get; }
    public IServiceProvider ServiceProvider { get; }
    public IConfiguration Configuration { get; }
    public TOptions Options { get; }

    public IDictionary<string, object?> Data { get; init; } = new Dictionary<string, object?>();

    public ILogger<TApplication> Logger { get; init; }

    public Output Output { get; init; }
    public Input Input { get; init; }
    public DateTimeProvider DateTime { get; init; }
    public GuidProvider Guid { get; init; }
    public FileSystem FileSystem { get; init; }

    public static TBuilder CreateBuilder(Action<TBuilder>? configure = null)
        => CreateBuilder([], configure);
    public static TBuilder CreateBuilder(string[] args, Action<TBuilder>? configure = null) {
        var builder = Create.Instance<TBuilder>((object)args)!;
        configure?.Invoke(builder);
        return builder;
    }

    public int Run() => RunAsync().GetAwaiter().GetResult();
    public abstract Task<int> RunAsync();

    public async ValueTask DisposeAsync() {
        if (_isDisposed) return;
        await Dispose();
        GC.SuppressFinalize(this);
        _isDisposed = true;
    }
}
