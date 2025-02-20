namespace DotNetToolbox.ConsoleApplication;

public sealed class ShellApplication
    : ShellApplication<ApplicationSettings> {
    internal ShellApplication(string[] args, IServiceCollection services)
        : base(args, services) {
    }
}

public class ShellApplication<TSettings>
    : ShellApplication<ShellApplication<TSettings>, TSettings>
    where TSettings : ApplicationSettings, new() {
    internal ShellApplication(string[] args, IServiceCollection services)
        : base(args, services) {
    }
}

public abstract class ShellApplication<TApplication, TSettings>(string[] args, IServiceCollection services)
    : ShellApplication<TApplication, ShellApplicationBuilder<TApplication, TSettings>, TSettings>(args, services)
    where TApplication : ShellApplication<TApplication, TSettings>
    where TSettings : ApplicationSettings, new();

public abstract class ShellApplication<TApplication, TBuilder, TSettings>
    : ApplicationBase<TApplication, TBuilder, TSettings>,
      IRunAsShell
    where TApplication : ShellApplication<TApplication, TBuilder, TSettings>
    where TBuilder : ShellApplicationBuilder<TApplication, TBuilder, TSettings>
    where TSettings : ApplicationSettings, new() {
    protected ShellApplication(string[] args, IServiceCollection services)
        : base(args, services) {
        AddCommand<ClearScreenCommand>();
        if (Settings.UseDefaultHelp) AddCommand<HelpCommand>();
        AddCommand<ExitCommand>();
    }
    protected bool AllowMultiLine { get; set; }

    internal sealed override async Task Run(CancellationToken ct = default) {
        var result = await OnStart(ct).ConfigureAwait(false);
        ProcessResult(result);
        if (!result.IsSuccess) {
            Exit(1);
            return;
        }

        while (IsRunning && !ct.IsCancellationRequested)
            await ProcessInteraction(ct).ConfigureAwait(false);
    }

    protected override Task<IValidationResult> OnStart(CancellationToken ct = default) {
        Output.WriteLine(FullName);
        return Task.FromResult(Success());
    }

    protected virtual string GetPrePromptText() => string.Empty;

    private async Task<IValidationResult> ProcessUserInput(string input, CancellationToken ct) {
        var lines = input.Split(Output.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (AllowMultiLine && lines.Length > 1)
            return await ProcessFreeText(lines, ct).ConfigureAwait(false);
        var tokens = UserInputParser.Parse(input);
        var result = StartsWithCommand(tokens.FirstOrDefault())
                   ? await ProcessCommand(tokens, ct).ConfigureAwait(false)
                   : await ProcessFreeText(lines, ct).ConfigureAwait(false);
        if (!result.IsSuccess) {
            foreach (var error in result.Errors) {
                Output.WriteLine($"Validation error: {error}");
            }
            Output.WriteLine();
        }
        return result;
    }

    protected virtual Task<IValidationResult> ProcessFreeText(string[] lines, CancellationToken ct = default)
        => Task.FromResult(Success());

    protected virtual Task<IValidationResult> ProcessInteraction(CancellationToken ct = default) {
        Output.Write(GetPrePromptText());
        Output.WritePrompt();
        var input = Input.ReadLine() ?? string.Empty;
        return ProcessUserInput(input, ct);
    }

    private bool StartsWithCommand(string? firstWord)
        => Commands.Any(c => c.Name.Equals(firstWord, StringComparison.OrdinalIgnoreCase)
                          || c.Aliases.Contains(firstWord));
}
