namespace DotNetToolbox.ConsoleApplication.Commands;

public class BackCommand(IHasChildren parent)
    : Command<BackCommand>(parent, "Back", n => {
        n.Description = "Back";
        n.Help = "Return to the previous menu.";
    }) {
    protected override IValidationResult Execute() => Success();
}
