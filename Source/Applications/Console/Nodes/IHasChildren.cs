namespace DotNetToolbox.ConsoleApplication.Nodes;

public interface IHasChildren : INode {
    IMap Context { get; }

    ICollection<INode> Children { get; }
    IArgument[] Options { get; }
    IParameter[] Parameters { get; }
    ICommand[] Commands { get; }

    ICommand AddCommand(string name, Delegate action);
    ICommand AddCommand(string name, string alias, Delegate action);
    ICommand AddCommand(string name, string[] aliases, Delegate action);
    ICommand AddCommand<TChildCommand>() where TChildCommand : Command<TChildCommand>, ICommand;
    void AddCommand(ICommand command);

    IFlag AddFlag(string name, Delegate? action = null);
    IFlag AddFlag(string name, string alias, Delegate? action = null);
    IFlag AddFlag(string name, string[] aliases, Delegate? action = null);
    IFlag AddFlag<TFlag>() where TFlag : Flag<TFlag>, IFlag;
    void AddFlag(IFlag flag);

    IOption AddOption(string name);
    IOption AddOption(string name, string alias);
    IOption AddOption(string name, string[] aliases);
    IOption AddOption<TOption>() where TOption : Option<TOption>, IOption;
    void AddOption(IOption option);

    IParameter AddParameter(string name);
    IParameter AddParameter(string name, string defaultValue);
    IParameter AddParameter<TParameter>() where TParameter : Parameter<TParameter>, IParameter;
    void AddParameter(IParameter parameter);
}
