namespace DotNetToolbox.ConsoleApplication.Commands;

internal class ClearScreenCommand(IHasChildren parent)
    : Command<ClearScreenCommand>(parent, "ClearScreen", n => {
        n.Aliases = ["cls"];
        n.Description = "Clear the screen.";
    }) {
    protected override IValidationResult Execute() {
        Output.ClearScreen();
        return Success();
    }
}
