using DotNetToolbox.Validation;

namespace DotNetToolbox.AI.Tools;

public class Tool
    : Map, IValidatable {
    public Tool(string id,
                string name,
                string returnType,
                List<ToolArgument>? arguments = null,
                string? description = null) {
        Id = id;
        Name = name;
        ReturnType = returnType;
        Description = description;
        Arguments = arguments ?? [];
    }

    public string Id {
        get => (string)this[nameof(Id)];
        init => this[nameof(Id)] = value;
    }

    public string Name {
        get => (string)this[nameof(Name)];
        init => this[nameof(Name)] = value;
    }

    public string ReturnType {
        get => (string)this[nameof(ReturnType)];
        init => this[nameof(ReturnType)] = value;
    }

    public string? Description {
        get => (string?)this[nameof(Description)];
        init => this[nameof(Description)] = value;
    }

    public List<ToolArgument> Arguments {
        get => (List<ToolArgument>)this[nameof(Arguments)];
        init => this[nameof(Arguments)] = value;
    }

    public string Signature => $"{Name}({string.Join(",", Arguments.Select(p => p.Signature))}) -> {ReturnType}";

    public Result Validate(IMap? context = null)
        => Result.Success();
}
