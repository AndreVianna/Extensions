namespace DotNetToolbox.ConsoleApplication.Nodes;

public interface INode {
    IApplication Application { get; }
    IApplicationEnvironment Environment { get; }
    IInput Input { get; }
    IOutput Output { get; }
    ILogger Logger { get; }

    string Name { get; }
    string[] Aliases { get; }
    string Description { get; }
    string Help { get; }

    string Path => this switch {
        IApplication app => app.AssemblyName,
        IHasParent { Parent: not IRunAsShell } node => $"{node.Parent.Path} {Name}".Trim(),
        _ => Name,
    };
}
