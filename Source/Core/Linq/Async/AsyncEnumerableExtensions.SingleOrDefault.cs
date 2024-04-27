﻿// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System.Linq.Async;

public static partial class AsyncEnumerableExtensions {
    public static ValueTask<TItem?> SingleOrDefaultAsync<TItem>(this IAsyncQueryable<TItem> source, CancellationToken cancellationToken = default)
        => FindSingleOrDefault(source, _ => true, default, cancellationToken);

    public static ValueTask<TItem?> SingleOrDefaultAsync<TItem>(this IAsyncQueryable<TItem> source, Func<TItem, bool> predicate, CancellationToken cancellationToken = default)
        => FindSingleOrDefault(source, predicate, default, cancellationToken);

    public static ValueTask<TItem?> SingleOrDefaultAsync<TItem>(this IAsyncQueryable<TItem> source, TItem? defaultValue, CancellationToken cancellationToken = default)
        => FindSingleOrDefault(source, _ => true, defaultValue, cancellationToken);

    public static ValueTask<TItem?> SingleOrDefaultAsync<TItem>(this IAsyncQueryable<TItem> source, Func<TItem, bool> predicate, TItem? defaultValue, CancellationToken cancellationToken = default)
        => FindSingleOrDefault(source, predicate, defaultValue, cancellationToken);

    private static async ValueTask<TItem?> FindSingleOrDefault<TItem>(IAsyncQueryable<TItem> source, Func<TItem, bool> predicate, TItem? defaultValue, CancellationToken cancellationToken)
    {
        IsNotNull(predicate);
        await using var enumerator = IsNotNull(source).GetAsyncEnumerator(cancellationToken);
        var found = false;
        var result = defaultValue;
        while (await enumerator.MoveNextAsync().ConfigureAwait(false)) {
            if (!predicate(enumerator.Current)) continue;
            if (found) throw new InvalidOperationException("Sequence contains more than one element.");
            found  = true;
            result = enumerator.Current;
        }
        return result;
    }
}
