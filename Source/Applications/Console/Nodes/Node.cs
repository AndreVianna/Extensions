namespace DotNetToolbox.ConsoleApplication.Nodes;

public abstract class Node<TNode>
    : IHasParent
    where TNode : Node<TNode> {
    protected Node(IHasChildren parent, string name, Action<TNode>? configure = null) {
        Parent = parent;
        Application = FindRoot(this);
        Environment = Application.Environment;
        var factory = Application.Services.GetRequiredService<ILoggerFactory>();
        Logger = factory.CreateLogger<TNode>();
        Name = IsValid(name, IsValidName);
        configure?.Invoke((TNode)this);
        ItemsAreValid(Aliases, _isValidAlias);
    }

    public IApplication Application { get; }
    public IApplicationEnvironment Environment { get; }
    public IHasChildren Parent { get; }
    public string Name { get; }
    public string[] Aliases { get; set; } = [];

    public string Description {
        get;
        set {
            field = value;
            if (string.IsNullOrWhiteSpace(Help)) Help = field;
        }
    } = string.Empty;

    public string Help { get; set; } = string.Empty;

    public IInput Input => Environment.OperatingSystem.Input;
    public IOutput Output => Environment.OperatingSystem.Output;

    public ILogger Logger { get; }

    public override string ToString() {
        string[] aliases = [Name, .. Aliases];
        return $"{GetType().Name}: {string.Join(",", aliases)} => {Description}\n{Help}";
    }

    public virtual string ToHelp() {
        string[] aliases = [Name, .. Aliases];
        return $"{Name,-10}{string.Join(",", aliases),-10}{Help}";
    }

    private static IApplication FindRoot(INode node) {
        while (node is IHasParent hasParent) node = hasParent.Parent;
        return (IApplication)node;
    }

    private static bool IsValidName(string? name)
        => name?.Length > 1
        && char.IsLetter(name[0])
        && name[1..].All(c => char.IsLetterOrDigit(c) || "-_".Contains(c));

    private readonly Func<string?, bool> _isValidAlias = IsValidAlias;
    private static bool IsValidAlias(string? alias)
        => !string.IsNullOrEmpty(alias)
        && alias.All(c => char.IsLetterOrDigit(c) || "!?@#$%&".Contains(c));
}
