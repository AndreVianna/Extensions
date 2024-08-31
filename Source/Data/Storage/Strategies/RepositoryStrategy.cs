namespace DotNetToolbox.Data.Strategies;

public class RepositoryStrategy<TStrategy, TRepository, TItem, TKey>(Lazy<TRepository> repository)
    : RepositoryStrategy<TStrategy, TRepository, TItem>(repository)
    , IRepositoryStrategy<TItem, TKey>
    where TStrategy : RepositoryStrategy<TStrategy, TRepository, TItem, TKey>
    where TRepository : class, IQueryableRepository<TItem>
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

    public virtual Result Update(TItem updatedItem, IContext? validationContext = null)
        => throw new NotImplementedException();
    public virtual Result UpdateMany(IEnumerable<TItem> updatedItems, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result AddOrUpdate(TItem updatedItem, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result AddOrUpdateMany(IEnumerable<TItem> updatedItems, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result Patch(TKey key, Action<TItem> setItem, IContext? validationContext = null)
        => throw new NotImplementedException();
    public virtual Result PatchMany(IEnumerable<TKey> keys, Action<TItem> setItem, IContext? validationContext = null)
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
        if (!result.IsSuccess) throw new InvalidOperationException("Failed to generate next key.");
        LastUsedKey = result.Value;
        return LastUsedKey;
    }

    public virtual ValueTask<TItem?> FindByKeyAsync(TKey key, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> UpdateAsync(TItem updatedItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> UpdateManyAsync(IEnumerable<TItem> updatedItems, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddOrUpdateAsync(TItem updatedItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddOrUpdateManyAsync(IEnumerable<TItem> updatedItems, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchAsync(TKey key, Func<TItem, CancellationToken, Task> setItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> PatchManyAsync(IEnumerable<TKey> keys, Action<TItem> setItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> PatchManyAsync(IEnumerable<TKey> keys, Func<TItem, CancellationToken, Task> setItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> RemoveAsync(TKey key, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> RemoveManyAsync(IEnumerable<TKey> keys, CancellationToken ct = default)
        => throw new NotImplementedException();

    #endregion
}

public class RepositoryStrategy<TStrategy, TRepository, TItem>(Lazy<TRepository> repository)
    : IRepositoryStrategy<TItem>
    where TStrategy : RepositoryStrategy<TStrategy, TRepository, TItem>
    where TRepository : class, IQueryableRepository<TItem> {
    private bool _disposed;
    private readonly Lazy<TRepository> _repository = repository;

    public async ValueTask DisposeAsync() {
        if (!_disposed) {
            await DisposeAsyncCore().ConfigureAwait(false);
            _disposed = true;
        }
    }

    protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

    protected QueryableRepository<TItem> Repository
        => _repository.Value as QueryableRepository<TItem>
        ?? throw new InvalidOperationException($"Repository of type {typeof(TRepository).Name} is not assinable to 'QueryableRepository<{typeof(TItem).Name}>'.");

    #region Blocking

    public virtual Result Seed(IEnumerable<TItem> seed, bool preserveContent = false, IContext? validationContext = null)
        => throw new NotImplementedException();
    public virtual Result Load()
        => throw new NotImplementedException();

    public virtual TItem[] GetAll()
        => throw new NotImplementedException();
    public virtual Page<TItem> GetPage(uint pageIndex = 0, uint pageSize = DefaultPageSize)
        => throw new NotImplementedException();
    public virtual Chunk<TItem> GetChunk(Expression<Func<TItem, bool>>? isChunkStart = null, uint blockSize = DefaultBlockSize)
        => throw new NotImplementedException();

    public virtual TItem? Find(Expression<Func<TItem, bool>> predicate)
        => throw new NotImplementedException();

    public virtual Result<TItem> Create(Action<TItem> setItem, IContext? validationContext = null)
        => throw new NotImplementedException();
    public virtual Result Add(TItem newItem, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result AddMany(IEnumerable<TItem> newItems, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result Update(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result UpdateMany(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> updatedItems, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result AddOrUpdate(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result AddOrUpdateMany(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> items, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result Patch(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result PatchMany(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IContext? validationContext = null)
        => throw new NotImplementedException();

    public virtual Result Remove(Expression<Func<TItem, bool>> predicate)
        => throw new NotImplementedException();

    public virtual Result RemoveMany(Expression<Func<TItem, bool>> predicate)
        => throw new NotImplementedException();

    public virtual Result Clear()
        => throw new NotImplementedException();

    #endregion

    #region Async

    public virtual Task<Result> SeedAsync(IEnumerable<TItem> seed, bool preserveContent = false, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> LoadAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual ValueTask<TItem[]> GetAllAsync(CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual ValueTask<Page<TItem>> GetPageAsync(uint pageIndex = 0, uint pageSize = DefaultPageSize, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual ValueTask<Chunk<TItem>> GetChunkAsync(Expression<Func<TItem, bool>>? isChunkStart = null, uint blockSize = 20U, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual ValueTask<TItem?> FindAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result<TItem>> CreateAsync(Func<TItem, CancellationToken, Task> setItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();
    public virtual Task<Result> AddAsync(TItem newItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddManyAsync(IEnumerable<TItem> newItems, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> UpdateAsync(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddOrUpdateAsync(Expression<Func<TItem, bool>> predicate, TItem updatedItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> AddOrUpdateManyAsync(Expression<Func<TItem, bool>> predicate, IEnumerable<TItem> updatedItems, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchAsync(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchAsync(Expression<Func<TItem, bool>> predicate, Func<TItem, CancellationToken, Task> setItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchManyAsync(Expression<Func<TItem, bool>> predicate, Action<TItem> setItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> PatchManyAsync(Expression<Func<TItem, bool>> predicate, Func<TItem, CancellationToken, Task> setItem, IContext? validationContext = null, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> RemoveAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> RemoveManyAsync(Expression<Func<TItem, bool>> predicate, CancellationToken ct = default)
        => throw new NotImplementedException();

    public virtual Task<Result> ClearAsync(CancellationToken ct = default)
        => throw new NotImplementedException();

    #endregion
}
