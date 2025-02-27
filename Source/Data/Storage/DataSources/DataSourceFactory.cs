namespace DotNetToolbox.Data.DataSources;

internal class DataSourceFactory
    : IDataSourceFactory {
    public InMemoryDataSource<TItem, TKey> CreateInMemory<TItem, TKey>(IEnumerable<TItem>? data = null)
        where TItem : class, IEntity<TKey>, new()
        where TKey : notnull
        => [.. data ?? []];

    public DataSource<TStorage, TItem, TKey> CreateFromStorage<TStorage, TItem, TKey>(IEnumerable<TItem>? data = null)
        where TStorage : class, IStorage<TItem, TKey>
        where TItem : class, IEntity<TKey>, new()
        where TKey : notnull
        => InstanceFactory.Create<DataSource<TStorage, TItem, TKey>>(data ?? []);
    public TRepository Create<TRepository, TStrategy, TItem, TKey>(IEnumerable<TItem>? data = null)
        where TRepository : DataSource<TStrategy, TItem, TKey>
        where TStrategy : class, IStorage<TItem, TKey>
        where TItem : class, IEntity<TKey>, new()
        where TKey : notnull
        => InstanceFactory.Create<TRepository>(data ?? []);

    public InMemoryDataSource<TItem> CreateInMemory<TItem>(IEnumerable<TItem>? data = null)
        => [.. data ?? []];

    public DataSource<TStorage, TItem> CreateFromStorage<TStorage, TItem>(IEnumerable<TItem>? data = null)
        where TStorage : class, IStorage<TItem>
        => InstanceFactory.Create<DataSource<TStorage, TItem>>(data ?? []);
    public TRepository Create<TRepository, TStrategy, TItem>(IEnumerable<TItem>? data = null)
        where TRepository : DataSource<TStrategy, TItem>
        where TStrategy : class, IStorage<TItem>
        => InstanceFactory.Create<TRepository>(data ?? []);
}
