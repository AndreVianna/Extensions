namespace DotNetToolbox.Data.Repositories.Entity;

public interface IUpdatableEntityRepository<TItem, TKey>
    : IUpdatableValueObjectRepository<TItem>
    where TItem : IEntity<TKey>
    where TKey : notnull {

    #region Blocking

    void Patch(TKey key, Action<TItem> setItem);
    void Update(TItem updatedItem);
    void Remove(TKey key);

    #endregion

    #region Async

    Task UpdateAsync(TItem updatedItem, CancellationToken ct = default);
    Task PatchAsync(TKey key, Func<TItem, CancellationToken, Task> setItem, CancellationToken ct = default);
    Task RemoveAsync(TKey key, CancellationToken ct = default);

    #endregion
}