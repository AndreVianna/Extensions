namespace DotNetToolbox.Domain.Models;

public interface ITrackChange {
    DateTimeOffset DateTime { get; }
    EntityAction Action { get; }
}
