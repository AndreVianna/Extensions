namespace DotNetToolbox.Data.DataSources;

[SuppressMessage("Design", "CA1010:Generic interface should also be implemented", Justification = "Implemented below")]
[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
public interface IQueryableDataSource
    : IAsyncQueryable;

public interface IQueryableDataSource<TItem>
    : IQueryableDataSource,
      IAsyncQueryable<TItem> {
    List<TItem> Records { get; }
}
