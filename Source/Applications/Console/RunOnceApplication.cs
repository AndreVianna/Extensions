namespace DotNetToolbox.ConsoleApplication;

public sealed class RunOnceApplication
    : RunOnceApplication<ApplicationSettings> {
    internal RunOnceApplication(string[] args, IServiceCollection services)
        : base(args, services) {
    }
}

public class RunOnceApplication<TSettings>
    : RunOnceApplication<RunOnceApplication<TSettings>, TSettings>
    where TSettings : ApplicationSettings, new() {
    internal RunOnceApplication(string[] args, IServiceCollection services)
        : base(args, services) {
    }
}

public abstract class RunOnceApplication<TApplication, TSettings>(string[] args, IServiceCollection services)
    : RunOnceApplication<TApplication, RunOnceApplicationBuilder<TApplication, TSettings>, TSettings>(args, services)
    where TApplication : RunOnceApplication<TApplication, TSettings>
    where TSettings : ApplicationSettings, new();

public abstract class RunOnceApplication<TApplication, TBuilder, TSettings>(string[] args, IServiceCollection services)
    : ApplicationBase<TApplication, TBuilder, TSettings>(args, services), IRunOnce
    where TApplication : RunOnceApplication<TApplication, TBuilder, TSettings>
    where TBuilder : RunOnceApplicationBuilder<TApplication, TBuilder, TSettings>
    where TSettings : ApplicationSettings, new() {
    internal sealed override async Task Run(CancellationToken ct = default) {
        if (Arguments.Length == 0) {
            await ShowHelp(ct).ConfigureAwait(false);
            return;
        }

        var result = await ExecuteDefault(ct).ConfigureAwait(false);
        ProcessResult(result);
    }

    protected virtual Task<Result> ExecuteDefault(CancellationToken ct = default) => Task.FromResult(Success());

    private Task<Result> ShowHelp(CancellationToken ct) {
        var help = new HelpCommand(this);
        return help.Execute([], ct);
    }
}
