namespace DotNetToolbox.Data.Storages;

public abstract class Storage<TStorage, TItem, TKey>(IList<TItem>? data = null)
    : Storage<TStorage, TItem>(data)
    , IStorage<TItem, TKey>
    where TStorage : Storage<TStorage, TItem, TKey>
    where TItem : IEntity<TKey>
    where TKey : notnull {
    #region Blocking

    protected virtual TKey FirstKey { get; } = default!;
    protected virtual TKey? LastUsedKey { get; set; }

    protected virtual Result LoadLastUsedKey()
        => throw new NotImplementedException();

    protected virtual bool TryGenerateNextKey([MaybeNullWhen(false)] out TKey next)
        => throw new NotImplementedException();

    protected bool TryGetNextKey([MaybeNullWhen(false)] out TKey next) {
        if (!TryGenerateNextKey(out next)) return false;
        LastUsedKey = next;
        return true;
    }

    public virtual TItem? FindByKey(TKey key)
        => throw new NotImplementedException();

    public virtual Result Update(TItem updatedItem, IMap? context = null)
        => throw new NotImplementedException();
    public virtual Result UpdateMany(IEnumerable<TItem> updatedItems, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result AddOrUpdate(TItem updatedItem, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result AddOrUpdateMany(IEnumerable<TItem> updatedItems, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result Patch(TKey key, Action<TItem> setItem, IMap? validationContext = null)
        => throw new NotImplementedException();
    public virtual Result PatchMany(IEnumerable<TKey> keys, Action<TItem> setItem, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result Remove(TKey key)
        => throw new NotImplementedException();
    public virtual Result RemoveMany(IEnumerable<TKey> keys)
        => throw new NotImplementedException();

    #endregion

    #region Async

    protected virtual Task<Result> LoadLastUsedKeyAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    protected virtual Task<Result<TKey>> GenerateNextKeyAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    protected async Task<Result<TKey>> GetNextKeyAsync(CancellationToken ct = default) {
        var result = await GenerateNextKeyAsync(ct);
        if (!result.IsSuccessful) throw new InvalidOperationException("Failed to generate next key.");
        LastUsedKey = result.Value;
        return LastUsedKey;
    }

    public virtual ValueTask<TItem?> FindByKeyAsync(TKey key, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> UpdateAsync(TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> UpdateManyAsync(IEnumerable<TItem> updatedItems, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddOrUpdateAsync(TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddOrUpdateManyAsync(IEnumerable<TItem> updatedItems, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchAsync(TKey key, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> PatchManyAsync(IEnumerable<TKey> keys, Action<TItem> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> PatchManyAsync(IEnumerable<TKey> keys, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> RemoveAsync(TKey key, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> RemoveManyAsync(IEnumerable<TKey> keys, CancellationToken ct = default)
        => throw new NotImplementedException();

    #endregion
}

public abstract class Storage<TStrategy, TItem>(IEnumerable<TItem>? data = null)
    : IStorage<TItem>
    where TStrategy : Storage<TStrategy, TItem> {
    private bool _disposed;

    public async ValueTask DisposeAsync() {
        if (!_disposed) {
            await DisposeAsyncCore().ConfigureAwait(false);
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

    public List<TItem> Data { get; } = data?.ToList() ?? [];

    #region Blocking

    public virtual Result Seed(IEnumerable<TItem> seed, bool preserveContent = false, IMap? validationContext = null)
        => throw new NotImplementedException();
    public virtual Result Load()
        => throw new NotImplementedException();

    public virtual TItem[] GetAll(Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null)
        => throw new NotImplementedException();
    public virtual Page<TItem> GetPage(uint pageIndex = 0, uint pageSize = DefaultPageSize, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null)
        => throw new NotImplementedException();
    public virtual Chunk<TItem> GetChunk(Expression<Func<TItem, bool>>? isChunkStart = null, uint blockSize = DefaultBlockSize, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null)
        => throw new NotImplementedException();

    public virtual TItem? Find(Expression<Func<TItem, bool>> predicate)
        => throw new NotImplementedException();

    public virtual Result<TItem> Create(Action<TItem>? setItem = null, IMap? validationContext = null)
        => throw new NotImplementedException();
    public virtual Result Add(TItem newItem, IMap? context = null)
        => throw new NotImplementedException();

    public virtual Result AddMany(IEnumerable<TItem> newItems, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result Update(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result UpdateMany(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> updatedItems, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result AddOrUpdate(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result AddOrUpdateMany(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> items, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result Patch(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result PatchMany(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result Remove(Expression<Func<TItem, bool>> predicate)
        => throw new NotImplementedException();

    public virtual Result RemoveMany(Expression<Func<TItem, bool>> predicate)
        => throw new NotImplementedException();

    public virtual Result Clear()
        => throw new NotImplementedException();

    #endregion

    #region Async

    public virtual Task<Result> SeedAsync(IEnumerable<TItem> seed, bool preserveContent = false, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> LoadAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual ValueTask<TItem[]> GetAllAsync(Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual ValueTask<Page<TItem>> GetPageAsync(uint pageIndex = 0, uint pageSize = DefaultPageSize, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual ValueTask<Chunk<TItem>> GetChunkAsync(Expression<Func<TItem, bool>>? isChunkStart = null, uint blockSize = 20U, Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual ValueTask<TItem?> FindAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result<TItem>> CreateAsync(Func<TItem, CancellationToken, Task> setItem, IMap validationContext, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<TItem> CreateAsync(Func<TItem, CancellationToken, Task> setItem, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> AddAsync(TItem newItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddManyAsync(IEnumerable<TItem> newItems, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> UpdateAsync(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddOrUpdateAsync(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddOrUpdateManyAsync(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> updatedItems, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchAsync(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchAsync(Expression<Func<TItem, bool>> predicate, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchManyAsync(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchManyAsync(Expression<Func<TItem, bool>> predicate, Func<TItem, CancellationToken, Task> setItem, IMap? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> RemoveAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> RemoveManyAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> ClearAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    #endregion
}
