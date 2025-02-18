using DotNetToolbox.Validation;

namespace DotNetToolbox.AI.Jobs;

public class UserProfile
    : Map,
      IValidatable {
    public UserProfile(uint id) {
        Id = id;
        Name = "User";
        Language = "English";
        Facts = [];
    }

    public uint Id {
        get => (uint)this[nameof(Id)];
        init => this[nameof(Id)] = value;
    }

    public string Name {
        get => (string)this[nameof(Name)];
        init => this[nameof(Name)] = value;
    }

    public string Language {
        get => (string)this[nameof(Language)];
        init => this[nameof(Language)] = value;
    }

    public List<string> Facts {
        get => (List<string>)this[nameof(Facts)];
        init => this[nameof(Facts)] = value;
    }

    public Result Validate(IMap? context = null) => Result.Success();
}
