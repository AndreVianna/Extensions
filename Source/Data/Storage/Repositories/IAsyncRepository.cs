﻿namespace DotNetToolbox.Data.Repositories;

public interface IAsyncRepository<TItem>
    : IReadOnlyAsyncRepository<TItem> {
    Task Add(TItem newItem, CancellationToken ct = default);
    Task Update(Expression<Func<TItem, bool>> predicate, TItem updatedItem, CancellationToken ct = default);
    Task Remove(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default);
}