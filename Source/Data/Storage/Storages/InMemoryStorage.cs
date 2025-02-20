using DotNetToolbox.Validation;

namespace DotNetToolbox.Data.Storages;

public class InMemoryStorage<TItem, TKey>(IList<TItem>? data = null)
    : Storage<InMemoryStorage<TItem, TKey>, TItem, TKey>(data)
    where TItem : class, IEntity<TKey>, new()
    where TKey : notnull {
    private readonly IStorage<TItem> _keylessStrategy = new InMemoryStorage<TItem>(data);

    #region Blocking

    public override Result Seed(IEnumerable<TItem> seed, bool preserveContent = false, IMap? validationContext = null)
        => _keylessStrategy.Seed(seed, preserveContent, validationContext);

    public override Result Load() {
        var result = _keylessStrategy.Load();
        result += LoadLastUsedKey();
        return result;
    }

    protected override Result LoadLastUsedKey() {
        LastUsedKey = Data.Count != 0
            ? Data.Max(item => item.Id)
            : default;
        return Result.Success(LastUsedKey);
    }

    public override TItem[] GetAll(Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null)
        => _keylessStrategy.GetAll(filterBy, orderBy);
    public override Page<TItem> GetPage(uint pageIndex = 0, uint pageSize = 20, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null)
        => _keylessStrategy.GetPage(pageIndex, pageSize, filterBy, orderBy);
    public override Chunk<TItem> GetChunk(Expression<Func<TItem, bool>>? isChunkStart = null, uint blockSize = 20, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null)
        => _keylessStrategy.GetChunk(isChunkStart, blockSize, filterBy, orderBy);

    public override TItem? Find(Expression<Func<TItem, bool>> predicate)
        => _keylessStrategy.Find(predicate);
    public override TItem? FindByKey(TKey key)
        => Find(x => x.Id.Equals(key));

    public override Result<TItem> Create(Action<TItem>? setItem = null, IMap? validationContext = null) {
        var item = new TItem();
        item.Id = TryGetNextKey(out var next) ? next : item.Id;
        setItem?.Invoke(item);
        var result = Result.Success(item);
        result += _keylessStrategy.Add(item, validationContext);
        return result;
    }

    public override Result Add(TItem newItem, IMap? context = null) {
        newItem.Id = TryGetNextKey(out var next) ? next : newItem.Id;
        return _keylessStrategy.Add(newItem, context);
    }

    public override Result Update(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null)
        => _keylessStrategy.Update(predicate, updatedItem, validationContext);

    public override Result Update(TItem updatedItem, IMap? context = null)
        => Update(x => x.Id.Equals(updatedItem.Id), updatedItem, context);

    public override Result Patch(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null)
        => _keylessStrategy.Patch(predicate, setItem, validationContext);
    public override Result Patch(TKey key, Action<TItem> setItem, IMap? validationContext = null)
        => Patch(x => x.Id.Equals(key), setItem, validationContext);

    public override Result Remove(Expression<Func<TItem, bool>> predicate)
        => _keylessStrategy.Remove(predicate);
    public override Result Remove(TKey key)
        => Remove(x => x.Id.Equals(key));

    public override Result AddMany(IEnumerable<TItem> newItems, IMap? validationContext = null)
        => _keylessStrategy.AddMany(newItems, validationContext);

    public override Result UpdateMany(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> updatedItems, IMap? validationContext = null)
        => _keylessStrategy.UpdateMany(predicate, updatedItems, validationContext);

    public override Result AddOrUpdate(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null)
        => _keylessStrategy.AddOrUpdate(predicate, updatedItem, validationContext);

    public override Result AddOrUpdateMany(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> items, IMap? validationContext = null)
        => _keylessStrategy.AddOrUpdateMany(predicate, items, validationContext);

    public override Result PatchMany(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null)
        => _keylessStrategy.PatchMany(predicate, setItem, validationContext);

    public override Result RemoveMany(Expression<Func<TItem, bool>> predicate)
        => _keylessStrategy.RemoveMany(predicate);

    public override Result Clear()
        => _keylessStrategy.Clear();

    public override Result UpdateMany(IEnumerable<TItem> updatedItems, IMap? validationContext = null) {
        var result = Result.Success();
        foreach (var updatedItem in updatedItems)
            result += Update(updatedItem, validationContext);
        return result;
    }

    public override Result AddOrUpdate(TItem updatedItem, IMap? validationContext = null) {
        var result = updatedItem.Validate(validationContext);
        if (!result.IsSuccess) return result;
        result += Remove(updatedItem.Id);
        return !result.IsSuccess
            ? result
            : _keylessStrategy.Add(updatedItem, validationContext);
    }

    public override Result AddOrUpdateMany(IEnumerable<TItem> updatedItems, IMap? validationContext = null) {
        var result = Result.Success();
        foreach (var item in updatedItems)
            result += AddOrUpdate(item, validationContext);
        return result;
    }

    public override Result PatchMany(IEnumerable<TKey> keys, Action<TItem> setItem, IMap? validationContext = null) {
        var result = Result.Success();
        foreach (var key in keys) {
            var item = FindByKey(key);
            if (item is null) {
                result += new ResultError($"Item with key {key} not found.", nameof(keys));
                continue;
            }
            setItem(item);
            result += item.Validate(validationContext);
        }
        return result;
    }

    public override Result RemoveMany(IEnumerable<TKey> keys) {
        var result = Result.Success();
        foreach (var key in keys)
            result += Remove(key);
        return result;
    }

    #endregion

    #region Async

    protected override async Task<Result> LoadLastUsedKeyAsync(CancellationToken ct = default) {
        LastUsedKey = await Data.AsAsyncQueryable().AnyAsync(ct)
            ? await Data.AsAsyncQueryable().MaxAsync(item => item.Id, ct)
            : default;
        return Result.Success(LastUsedKey);
    }

    public override Task<Result> SeedAsync(IEnumerable<TItem> seed, bool preserveContent = false, IMap? validationContext = null, CancellationToken ct = default)
        => _keylessStrategy.SeedAsync(seed, preserveContent, validationContext, ct);
    public override async Task<Result> LoadAsync(CancellationToken ct = default) {
        var result = await _keylessStrategy.LoadAsync(ct);
        result += await LoadLastUsedKeyAsync(ct);
        return result;
    }

    public override ValueTask<TItem[]> GetAllAsync(Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null, CancellationToken ct = default)
        => _keylessStrategy.GetAllAsync(filterBy, orderBy, ct);
    public override ValueTask<Page<TItem>> GetPageAsync(uint pageIndex = 0, uint pageSize = 20, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null, CancellationToken ct = default)
        => _keylessStrategy.GetPageAsync(pageIndex, pageSize, filterBy, orderBy, ct);
    public override ValueTask<Chunk<TItem>> GetChunkAsync(Expression<Func<TItem, bool>>? isChunkStart = null, uint blockSize = 20, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null, CancellationToken ct = default)
        => _keylessStrategy.GetChunkAsync(isChunkStart, blockSize, filterBy, orderBy, ct);

    public override ValueTask<TItem?> FindAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => _keylessStrategy.FindAsync(predicate, ct);
    public override ValueTask<TItem?> FindByKeyAsync(TKey key, CancellationToken ct = default)
        => FindAsync(x => x.Id.Equals(key), ct);

    public override async Task<Result<TItem>> CreateAsync(Func<TItem, CancellationToken, Task> setItem, IMap validationContext, CancellationToken ct = default) {
        var item = new TItem();
        await setItem(item, ct);
        var result = Result.Success(item);
        result += item.Validate(validationContext);
        return result;
    }

    public override async Task<TItem> CreateAsync(Func<TItem, CancellationToken, Task> setItem, CancellationToken ct = default) {
        var item = new TItem();
        await setItem(item, ct);
        return item;
    }

    public override async Task<Result> AddAsync(TItem newItem, IMap? validationContext = null, CancellationToken ct = default) {
        newItem.Id = await GetNextKeyAsync(ct);
        return await _keylessStrategy.AddAsync(newItem, validationContext, ct);
    }

    public override Task<Result> UpdateAsync(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default)
        => _keylessStrategy.UpdateAsync(predicate, updatedItem, validationContext, ct);
    public override Task<Result> UpdateAsync(TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default)
        => UpdateAsync(x => x.Id.Equals(updatedItem.Id), updatedItem, validationContext, ct);

    public override Task<Result> PatchAsync(Expression<Func<TItem, bool>> predicate, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => _keylessStrategy.PatchAsync(predicate, setItem, validationContext, ct);
    public override Task<Result> PatchAsync(TKey key, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => PatchAsync(x => x.Id.Equals(key), setItem, validationContext, ct);

    public override Task<Result> RemoveAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => _keylessStrategy.RemoveAsync(predicate, ct);
    public override Task<Result> RemoveAsync(TKey key, CancellationToken ct = default)
        => RemoveAsync(x => x.Id.Equals(key), ct);

    public override Task<Result> AddManyAsync(IEnumerable<TItem> newItems, IMap? validationContext = null, CancellationToken ct = default)
        => _keylessStrategy.AddManyAsync(newItems, validationContext, ct);
    public override Task<Result> AddOrUpdateAsync(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default)
        => _keylessStrategy.AddOrUpdateAsync(predicate, updatedItem, validationContext, ct);
    public override Task<Result> AddOrUpdateManyAsync(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> updatedItems, IMap? validationContext = null, CancellationToken ct = default)
        => _keylessStrategy.AddOrUpdateManyAsync(predicate, updatedItems, validationContext, ct);
    public override Task<Result> PatchAsync(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => _keylessStrategy.PatchAsync(predicate, setItem, validationContext, ct);
    public override Task<Result> PatchManyAsync(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => _keylessStrategy.PatchManyAsync(predicate, setItem, validationContext, ct);
    public override Task<Result> PatchManyAsync(Expression<Func<TItem, bool>> predicate, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => _keylessStrategy.PatchManyAsync(predicate, setItem, validationContext, ct);
    public override Task<Result> RemoveManyAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => _keylessStrategy.RemoveManyAsync(predicate, ct);
    public override Task<Result> ClearAsync(CancellationToken ct = default)
        => _keylessStrategy.ClearAsync(ct);

    public override async Task<Result> UpdateManyAsync(IEnumerable<TItem> updatedItems, IMap? validationContext = null, CancellationToken ct = default) {
        var result = Result.Success();
        await foreach (var updatedItem in updatedItems.AsAsyncEnumerable(ct))
            result += await UpdateAsync(updatedItem, validationContext, ct);
        return result;
    }

    public override async Task<Result> AddOrUpdateAsync(TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default) {
        var result = updatedItem.Validate(validationContext);
        if (!result.IsSuccess) return result;
        result += await RemoveAsync(updatedItem.Id, ct);
        return !result.IsSuccess
            ? result
            : await AddAsync(updatedItem, validationContext, ct);
    }

    public override async Task<Result> AddOrUpdateManyAsync(IEnumerable<TItem> updatedItems, IMap? validationContext = null, CancellationToken ct = default) {
        var result = Result.Success();
        await foreach (var item in updatedItems.AsAsyncEnumerable(ct)) result += await AddOrUpdateAsync(item, validationContext, ct);
        return result;
    }

    public override async Task<Result> PatchManyAsync(IEnumerable<TKey> keys, Action<TItem> setItem, IMap? validationContext = null, CancellationToken ct = default) {
        var result = Result.Success();
        await foreach (var key in keys.AsAsyncEnumerable(ct)) {
            var item = await FindByKeyAsync(key, ct: ct);
            if (item is null) {
                result += new ResultError($"Item with key {key} not found.", nameof(keys));
                continue;
            }
            setItem(item);
            result += item.Validate(validationContext);
        }
        return result;
    }
    public override async Task<Result> PatchManyAsync(IEnumerable<TKey> keys, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default) {
        var result = Result.Success();
        await foreach (var key in keys.AsAsyncEnumerable(ct)) {
            var item = await FindByKeyAsync(key, ct: ct);
            if (item is null) {
                result += new ResultError($"Item with key {key} not found.", nameof(keys));
                continue;
            }
            await setItem(item, ct);
            result += item.Validate(validationContext);
        }
        return result;
    }

    public override async Task<Result> RemoveManyAsync(IEnumerable<TKey> keys, CancellationToken ct = default) {
        var result = Result.Success();
        await foreach (var key in keys.AsAsyncEnumerable(ct)) {
            var item = await FindByKeyAsync(key, ct: ct);
            if (item is null) {
                result += new ResultError($"Item with key {key} not found.", nameof(keys));
                continue;
            }
            Data.Remove(item);
        }
        return result;
    }

    #endregion
}

public class InMemoryStorage<TItem>(IList<TItem>? data = null)
    : Storage<InMemoryStorage<TItem>, TItem>(data) {
    #region Blocking

    public override Result Seed(IEnumerable<TItem> seed, bool preserveContent = false, IMap? validationContext = null) {
        var result = Result.Success();
        if (!preserveContent) result += Clear();
        result += AddMany(seed, validationContext);
        return result;
    }

    public override Result Load() => Result.Success();

    public override TItem[] GetAll(Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null)
        => [.. Data];

    public override Page<TItem> GetPage(uint pageIndex = 0, uint pageSize = DefaultPageSize, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null) {
        var count = Data.Count;
        var items = Data.Skip((int)(pageIndex * pageSize))
                   .Take((int)pageSize)
                   .ToArray();
        return new() {
            TotalCount = (uint)count,
            Index = pageIndex,
            Size = pageSize,
            Items = items,
        };
    }

    public override Chunk<TItem> GetChunk(Expression<Func<TItem, bool>>? isChunkStart = null, uint blockSize = DefaultBlockSize, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null) {
        var query = Data.AsQueryable();
        if (isChunkStart is not null) {
            var isNotStart = (Expression<Func<TItem, bool>>)Expression.Lambda(Expression.Not(isChunkStart.Body), isChunkStart.Parameters);
            query = query.SkipWhile(isNotStart);
        }
        var items = query
                   .Take((int)blockSize)
                   .ToArray();
        return new() {
            Size = blockSize,
            Items = items,
        };
    }

    public override TItem? Find(Expression<Func<TItem, bool>> predicate)
        => Data.AsQueryable().FirstOrDefault(predicate);

    public override Result<TItem> Create(Action<TItem>? setItem = null, IMap? validationContext = null) {
        var item = Activator.CreateInstance<TItem>();
        setItem?.Invoke(item);
        var result = Result.Success(item);
        result += Add(item, validationContext);
        return result;
    }

    public override Result Add(TItem newItem, IMap? context = null) {
        var result = Result.Success();
        result = newItem is IValidatable validatable
            ? result + validatable.Validate(context)
            : result;
        if (result.IsSuccess) Data.Add(newItem);
        return result;
    }
    public override Result AddMany(IEnumerable<TItem> newItems, IMap? validationContext = null) {
        var result = Result.Success();
        var validItems = new List<TItem>();
        foreach (var newItem in newItems) {
            var itemResult = Result.Success();
            if (newItem is IValidatable validatable) itemResult += validatable.Validate(validationContext);
            if (!itemResult.IsSuccess) {
                result += itemResult;
                continue;
            }
            validItems.Add(newItem);
        }

        Data.AddRange(validItems);
        return result;
    }

    public override Result Update(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null) {
        var result = TryRemove(predicate);
        return !result.IsSuccess
            ? result
            : Add(updatedItem, validationContext);
    }
    public override Result AddOrUpdate(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null) {
        Remove(predicate);
        return Add(updatedItem, validationContext);
    }
    public override Result AddOrUpdateMany(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> items, IMap? validationContext = null) {
        Remove(predicate);
        return AddMany(items, validationContext);
    }

    public override Result Patch(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null) {
        var itemToPatch = Data.AsQueryable().FirstOrDefault(predicate);
        if (itemToPatch is null) return Result.Invalid("Item not found.", nameof(predicate));
        setItem(itemToPatch);
        return itemToPatch is IValidatable validatable
            ? validatable.Validate(validationContext)
            : Result.Success();
    }
    public override Result PatchMany(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null) {
        var itemsToPatch = Data.AsQueryable().Where(predicate);
        var result = Result.Success();
        foreach (var item in itemsToPatch) {
            setItem(item);
            if (item is IValidatable validatable)
                result += validatable.Validate(validationContext);
        }
        return result;
    }

    public override Result Remove(Expression<Func<TItem, bool>> predicate)
        => TryRemove(predicate);
    public override Result RemoveMany(Expression<Func<TItem, bool>> predicate) {
        var itemsToRemove = Data.AsQueryable().Where(predicate);
        foreach (var item in itemsToRemove.ToArray()) Data.Remove(item);
        return Result.Success();
    }

    public override Result Clear() {
        Data.Clear();
        return Result.Success();
    }

    private Result TryRemove(Expression<Func<TItem, bool>> predicate) {
        var itemToRemove = Data.AsQueryable().FirstOrDefault(predicate);
        if (itemToRemove is null)
            return new ResultError("Item not found.", nameof(predicate));
        Data.Remove(itemToRemove);
        return Result.Success();
    }

    #endregion

    #region Async

    public override Task<Result> SeedAsync(IEnumerable<TItem> seed, bool preserveContent = false, IMap? validationContext = null, CancellationToken ct = default) {
        Seed(seed, preserveContent, validationContext);
        return Result.SuccessTask();
    }

    public override Task<Result> LoadAsync(CancellationToken ct = default) {
        Load();
        return Result.SuccessTask();
    }

    public override ValueTask<TItem[]> GetAllAsync(Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null, CancellationToken ct = default) {
        var query = Data.AsAsyncQueryable();
        query = ApplyFilter(query, filterBy);
        query = ApplySorting(query, orderBy);
        return query.ToArrayAsync(ct);
    }

    private static IAsyncQueryable<TItem> ApplyFilter(IAsyncQueryable<TItem> query, Expression<Func<TItem, bool>>? filterBy = null)
     => filterBy is null ? query : query.Where(filterBy);

    private static IAsyncQueryable<TItem> ApplySorting(IAsyncQueryable<TItem> query, HashSet<SortClause>? orderBy = null) {
        if (orderBy is null) return query;
        IOrderedAsyncQueryable<TItem>? orderedQuery = null;

        foreach (var clause in orderBy) {
            if (typeof(TItem).GetProperty(clause.PropertyName) is null)
                throw new ArgumentException($"Property {clause.PropertyName} not found on {typeof(TItem).Name}.", nameof(orderBy));

            var parameter = Expression.Parameter(typeof(TItem), "x");
            var property = Expression.Property(parameter, clause.PropertyName);
            var lambda = Expression.Lambda<Func<TItem, object>>(property, parameter);
            orderedQuery = orderedQuery is null
                ? clause.Direction is SortDirection.Ascending
                    ? query.OrderBy(lambda)
                    : query.OrderByDescending(lambda)
                : clause.Direction is SortDirection.Ascending
                    ? orderedQuery.ThenBy(lambda)
                    : orderedQuery.ThenByDescending(lambda);
        }
        return orderedQuery ?? query;
    }

    public override async ValueTask<Page<TItem>> GetPageAsync(uint pageIndex = 0, uint pageSize = DefaultPageSize, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null, CancellationToken ct = default) {
        var count = await Data.AsAsyncQueryable().CountAsync(ct);
        var items = await Data.AsAsyncQueryable().Skip((int)(pageIndex * pageSize))
                   .Take((int)pageSize)
                   .ToArrayAsync(ct);
        return new() {
            TotalCount = (uint)count,
            Index = pageIndex,
            Size = pageSize,
            Items = items,
        };
    }

    public override async ValueTask<Chunk<TItem>> GetChunkAsync(Expression<Func<TItem, bool>>? isChunkStart = null, uint blockSize = DefaultBlockSize, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null, CancellationToken ct = default) {
        var query = Data.AsAsyncQueryable();
        if (isChunkStart is not null) {
            var isNotStart = (Expression<Func<TItem, bool>>)Expression.Lambda(Expression.Not(isChunkStart.Body), isChunkStart.Parameters);
            query = query.SkipWhile(isNotStart).AsAsyncQueryable();
        }
        var items = await query
                   .Take((int)blockSize)
                   .ToArrayAsync(ct);
        return new() {
            Size = blockSize,
            Items = items,
        };
    }

    public override ValueTask<TItem?> FindAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => Data.AsAsyncQueryable().FirstOrDefaultAsync(predicate, ct);

    public override async Task<Result<TItem>> CreateAsync(Func<TItem, CancellationToken, Task> setItem, IMap validationContext, CancellationToken ct = default) {
        var item = Activator.CreateInstance<TItem>();
        await setItem(item, ct);
        var result = Result.Success(item);
        if (item is IValidatable validatable) result += validatable.Validate(validationContext);
        return result;
    }

    public override Task<Result> AddAsync(TItem newItem, IMap? validationContext = null, CancellationToken ct = default)
        => Task.Run(() => Add(newItem), ct);
    public override async Task<Result> AddManyAsync(IEnumerable<TItem> newItems, IMap? validationContext = null, CancellationToken ct = default) {
        var result = Result.Success();
        await foreach (var item in newItems.AsAsyncEnumerable(ct)) {
            var itemResult = Result.Success();
            if (item is IValidatable validatable) itemResult += validatable.Validate(validationContext);
            if (!itemResult.IsSuccess) {
                result += itemResult;
                continue;
            }
            Data.Add(item);
        }
        return result;
    }

    public override async Task<Result> UpdateAsync(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default) {
        var result = await TryRemoveAsync(predicate, ct);
        return !result.IsSuccess
            ? result
            : await AddAsync(updatedItem, validationContext, ct);
    }

    public override async Task<Result> AddOrUpdateAsync(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default) {
        await RemoveAsync(predicate, ct);
        return await AddAsync(updatedItem, validationContext, ct);
    }
    public override async Task<Result> AddOrUpdateManyAsync(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> updatedItems, IMap? validationContext = null, CancellationToken ct = default) {
        await RemoveAsync(predicate, ct);
        return await AddManyAsync(updatedItems, validationContext, ct);
    }

    public override async Task<Result> PatchAsync(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null, CancellationToken ct = default) {
        var itemToPatch = await Data.AsAsyncQueryable().FirstOrDefaultAsync(predicate, ct);
        if (itemToPatch is null) return Result.Invalid("Item not found.", nameof(predicate));
        setItem(itemToPatch);
        return itemToPatch is IValidatable validatable
            ? validatable.Validate(validationContext)
            : Result.Success();
    }
    public override async Task<Result> PatchAsync(Expression<Func<TItem, bool>> predicate, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default) {
        var itemToPatch = await Data.AsAsyncQueryable().FirstOrDefaultAsync(predicate, ct);
        if (itemToPatch is null) return Result.Invalid("Item not found.", nameof(predicate));
        await setItem(itemToPatch, ct);
        return itemToPatch is IValidatable validatable
            ? validatable.Validate(validationContext)
            : Result.Success();
    }

    public override async Task<Result> PatchManyAsync(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null, CancellationToken ct = default) {
        var itemsToPatch = Data.AsAsyncQueryable().Where(predicate).AsAsyncEnumerable(ct);
        var result = Result.Success();
        await foreach (var item in itemsToPatch) {
            setItem(item);
            if (item is IValidatable validatable)
                result += validatable.Validate(validationContext);
        }
        return result;
    }
    public override async Task<Result> PatchManyAsync(Expression<Func<TItem, bool>> predicate, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default) {
        var itemsToPatch = Data.AsAsyncQueryable().Where(predicate).AsAsyncEnumerable(ct);
        var result = Result.Success();
        await foreach (var item in itemsToPatch) {
            await setItem(item, ct);
            if (item is IValidatable validatable)
                result += validatable.Validate(validationContext);
        }
        return result;
    }

    public override Task<Result> RemoveAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => TryRemoveAsync(predicate, ct);
    public override async Task<Result> RemoveManyAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default) {
        await foreach (var item in Data.AsAsyncQueryable().Where(predicate).AsAsyncEnumerable(ct)) Data.Remove(item);
        return Result.Success();
    }

    public override Task<Result> ClearAsync(CancellationToken ct = default)
        => Task.Run(Clear, ct);

    private async Task<Result> TryRemoveAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default) {
        var itemToRemove = await Data.AsAsyncQueryable().FirstOrDefaultAsync(predicate, ct);
        if (itemToRemove is null)
            return new ResultError("Item not found.", nameof(predicate));
        Data.Remove(itemToRemove);
        return Result.Success();
    }

    #endregion
}
