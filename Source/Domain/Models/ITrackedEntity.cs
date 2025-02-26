namespace DotNetToolbox.Domain.Models;

public interface ITrackedEntity<TEntity, TChangeTracker>
    : IHaveHistory<TChangeTracker>,
      IEntity
    where TEntity : ITrackedEntity<TEntity, TChangeTracker>, IEntity
    where TChangeTracker : ITrackChange;
