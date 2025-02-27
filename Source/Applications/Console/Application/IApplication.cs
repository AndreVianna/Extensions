namespace DotNetToolbox.ConsoleApplication.Application;

public interface IApplication
    : IHasChildren,
      IAsyncDisposable {
    const int DefaultExitCode = 0;
    const int DefaultErrorCode = 1;
    int ExitCode { get; }

    string Version { get; }
    string DisplayVersion { get; }
    string AssemblyName { get; }
    string FullName { get; }
    IServiceProvider Services { get; }
    IConfigurationRoot Configuration { get; }

    void Exit(int code = DefaultExitCode);
}

public interface IApplication<out TSettings>
    : IApplication {
    TSettings Settings { get; }
}

public interface IApplication<TApplication, out TBuilder, out TSettings>
    : IApplication<TSettings>,
      IBuilderCreator<TApplication, TBuilder, TSettings>,
      IApplicationCreator<TApplication, TBuilder, TSettings>
    where TApplication : class, IApplication<TApplication, TBuilder, TSettings>
    where TBuilder : class, IApplicationBuilder<TApplication, TBuilder, TSettings>
    where TSettings : ApplicationSettings, new() {
    int Run();
    Task<int> RunAsync();
}
