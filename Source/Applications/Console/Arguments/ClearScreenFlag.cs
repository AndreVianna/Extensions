namespace DotNetToolbox.ConsoleApplication.Arguments;

internal sealed class ClearScreenFlag(IHasChildren parent)
    : Flag<ClearScreenFlag>(parent, "Clear-Screen", n => {
        n.Aliases = ["cls"];
        n.Description = "Clear the screen.";
    }) {
    protected override Task<Result> Execute(CancellationToken ct = default) {
        Output.ClearScreen();
        return Task.FromResult(Success());
    }
}
