namespace DotNetToolbox.ConsoleApplication.Nodes;

public sealed class Command(IHasChildren parent,
                            string name,
                            Action<Command>? configure = null,
                            Func<Command, CancellationToken, Task<Result>>? executeAsync = null)
    : Command<Command>(parent, name, configure, executeAsync) {
    public Command(IHasChildren parent, string name, Func<Command, Result> execute)
        : this(parent, name, null, (cmd, ct) => Task.Run(() => execute(cmd), ct)) {
    }
    public Command(IHasChildren parent, string name, Action<Command>? configure, Func<Command, Result> execute)
        : this(parent, name, configure, (cmd, ct) => Task.Run(() => execute(cmd), ct)) {
    }
}

public class Command<TCommand>(IHasChildren parent,
                               string name,
                               Action<TCommand>? configure = null,
                               Func<TCommand, CancellationToken, Task<Result>>? executeAsync = null)
    : Node<TCommand>(parent, name, configure),
      ICommand
    where TCommand : Command<TCommand> {
    public Command(IHasChildren parent, string name, Func<TCommand, Result> execute)
        : this(parent, name, null, (cmd, ct) => Task.Run(() => execute(cmd), ct)) {
    }

    public Command(IHasChildren parent, string name, Action<TCommand> configure, Func<TCommand, Result> execute)
        : this(parent, name, configure, (cmd, ct) => Task.Run(() => execute(cmd), ct)) {
    }

    public IMap Context { get; } = new Map();

    public ICollection<INode> Children { get; } = [];
    public IParameter[] Parameters => [.. Children.OfType<IParameter>().OrderBy(i => i.Order)];
    public IArgument[] Options => [.. Children.OfType<IArgument>()];
    public ICommand[] Commands => [.. Children.OfType<ICommand>().Except(Options.Cast<INode>()).Cast<ICommand>()];

    protected virtual Task<Result> ExecuteAsync(CancellationToken ct = default)
        => executeAsync?.Invoke((TCommand)this, ct) ?? Task.Run(Execute, ct);
    protected virtual Result Execute() => Success();
    public async Task<Result> Execute(IReadOnlyList<string> args, CancellationToken ct = default) {
        var result = await ArgumentsParser.Parse(this, args, ct);
        return result.IsSuccessful ? await ExecuteAsync(ct) : result;
    }

    public ICommand AddCommand(string name, Delegate action)
        => AddCommand(name, aliases: [], action);
    public ICommand AddCommand(string name, string alias, Delegate action)
        => AddCommand(name, [alias], action);
    public ICommand AddCommand(string name, string[] aliases, Delegate action)
        => NodeFactory.Create<Command>(this, name, (Action<Parameter>)(n => n.Aliases = aliases), action);
    public ICommand AddCommand<TChildCommand>()
        where TChildCommand : Command<TChildCommand>, ICommand
        => NodeFactory.Create<TChildCommand>(this);
    public void AddCommand(ICommand command) => Children.Add(command);

    public IFlag AddFlag(string name, Delegate? action = null)
        => AddFlag(name, aliases: [], action);
    public IFlag AddFlag(string name, string alias, Delegate? action = null)
        => AddFlag(name, [alias], action);
    public IFlag AddFlag(string name, string[] aliases, Delegate? action = null)
        => NodeFactory.Create<Flag>(this, name, (Action<Parameter>)(n => n.Aliases = aliases), action);
    public IFlag AddFlag<TFlag>()
        where TFlag : Flag<TFlag>, IFlag
        => NodeFactory.Create<TFlag>(this);
    public void AddFlag(IFlag flag) => Children.Add(flag);

    public IOption AddOption(string name)
        => AddOption(name, aliases: []);
    public IOption AddOption(string name, string alias)
        => AddOption(name, [alias]);
    public IOption AddOption(string name, string[] aliases)
        => NodeFactory.Create<Option>(this, name, (Action<Parameter>)(n => n.Aliases = aliases));
    public IOption AddOption<TOption>()
        where TOption : Option<TOption>, IOption
        => NodeFactory.Create<TOption>(this);
    public void AddOption(IOption option) => Children.Add(option);

    public IParameter AddParameter(string name)
        => AddParameter(name, null);
    public IParameter AddParameter(string name, string? defaultValue)
        => NodeFactory.Create<Parameter>(this, name, (Action<Parameter>)(n => n.DefaultValue = IsNotNull(defaultValue)));
    public IParameter AddParameter<TParameter>()
        where TParameter : Parameter<TParameter>, IParameter
        => NodeFactory.Create<TParameter>(this);
    public void AddParameter(IParameter parameter) => Children.Add(parameter);
}
