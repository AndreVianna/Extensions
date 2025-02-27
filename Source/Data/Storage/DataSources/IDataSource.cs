namespace DotNetToolbox.Data.DataSources;

[SuppressMessage("Design", "CA1010:Generic interface should also be implemented", Justification = "Implemented below")]
[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
public interface IDataSource
    : IQueryableDataSource
    , IReadOnlyDataSource
    , IUpdatableDataSource
    , IAsyncDisposable;

public interface IDataSource<TItem>
    : IDataSource
    , IQueryableDataSource<TItem>
    , IReadOnlyDataSource<TItem>
    , IUpdatableDataSource<TItem>;

public interface IDataSource<TItem, in TKey>
    : IDataSource<TItem>
    , IReadOnlyDataSource<TItem, TKey>
    , IUpdatableDataSource<TItem, TKey>
    where TItem : IEntity<TKey>
    where TKey : notnull;
