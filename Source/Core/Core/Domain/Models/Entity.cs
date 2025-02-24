
namespace DotNetToolbox.Domain.Models;

public abstract class Entity<TEntity, TKey>
    : IEntity<TKey>
    where TEntity : Entity<TEntity, TKey>, new()
    where TKey : notnull {
    public TKey Id { get; set; } = default!;

    public abstract IResult Validate(IMap? context = null);
}
