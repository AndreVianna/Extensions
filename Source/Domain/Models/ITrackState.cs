namespace DotNetToolbox.Domain.Models;

public interface ITrackState
    : ITrackState<object>;

public interface ITrackState<out TState>
    : ITrackChange {
    TState? State { get; }
}
