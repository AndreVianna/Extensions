namespace System.Linq.Async;

[Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1010:Generic interface should also be implemented", Justification = "Implemented below")]
[Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
public interface IAsyncQueryable
    : IQueryable {
    IAsyncQueryProvider AsyncProvider { get; }
}

public interface IAsyncQueryable<out TItem>
    : IAsyncEnumerable<TItem>
    , IAsyncQueryable
    , IQueryable<TItem>;
