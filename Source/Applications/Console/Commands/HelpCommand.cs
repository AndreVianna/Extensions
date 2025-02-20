namespace DotNetToolbox.ConsoleApplication.Commands;

internal sealed class HelpCommand(IHasChildren parent)
    : Command<HelpCommand>(parent, "Help", n => {
        n.Aliases = ["?"];
        n.Description = "Display this help information.";
        n.AddParameter("Target", string.Empty);
    }) {
    protected override IValidationResult Execute() {
        var target = Context.GetValueAs<string>("Target");
        var command = Parent.Commands.FirstOrDefault(i => i.Name.Equals(target, StringComparison.OrdinalIgnoreCase));
        var node = command ?? Parent;
        Output.WriteLine(node.ToHelp());
        return Success();
    }
}
