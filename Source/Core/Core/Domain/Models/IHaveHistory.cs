namespace DotNetToolbox.Domain.Models;

public interface IHaveHistory<TChangeTracker>
    where TChangeTracker : ITrackChange {
    List<TChangeTracker> History { get; }
}
