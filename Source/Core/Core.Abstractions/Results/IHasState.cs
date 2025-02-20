namespace DotNetToolbox.Results;

public interface IHasState<out TState> {
    TState State { get; }
}
