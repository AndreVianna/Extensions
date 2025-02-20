namespace DotNetToolbox.ConsoleApplication.Commands;

public class ExitCommand(IHasChildren parent)
    : Command<ExitCommand>(parent, "Exit", n => {
        n.Aliases = ["quit"];
        n.Description = "Exit";
        n.Help = "Exit the application.";
    }) {
    protected override IValidationResult Execute() {
        Application.Exit();
        return Success();
    }
}
