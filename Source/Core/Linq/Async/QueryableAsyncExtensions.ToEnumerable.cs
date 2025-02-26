// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System.Linq.Async;

public static partial class QueryableAsyncExtensions {
    public static IEnumerable<TSource> ToEnumerable<TSource>(this IAsyncQueryable<TSource> source) {
        var enumerable = source.GetAsyncEnumerator();
        try {
            while (enumerable.MoveNextAsync().AsTask().GetAwaiter().GetResult()) {
                yield return enumerable.Current;
            }
        }
        finally {
            enumerable.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }
}
