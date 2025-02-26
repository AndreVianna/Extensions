namespace System.Collections.Generic;

public static class EnumerableExtensions {
    public static IAsyncEnumerable<TItem> ToAsyncEnumerable<TItem>(this IEnumerable<TItem> source)
        => new AsyncEnumerable<TItem>(source);

    public static IAsyncEnumerator<TItem> GetAsyncEnumerator<TItem>(this IEnumerable<TItem> source, CancellationToken cancellationToken = default)
        => source.ToAsyncEnumerable().GetAsyncEnumerator(cancellationToken);

    #region Projections

    public static IEnumerable<TItem> As<TItem>(this IEnumerable source)
        => source.Cast<TItem>();
    public static IEnumerable<TNewItem> As<TItem, TNewItem>(this IEnumerable source, Func<TItem, TNewItem> convertTo)
        => source.As<TItem>().Select(convertTo);
    public static TItem[] ToArray<TItem>(this IEnumerable source)
        => [.. IsNotNull(source).As<TItem>()];
    public static TNewItem[] ToArray<TItem, TNewItem>(this IEnumerable<TItem> source, Func<TItem, TNewItem> convertTo)
        => [.. IsNotNull(source).Select(convertTo)];
    public static List<TItem> ToList<TItem>(this IEnumerable source)
        => [.. IsNotNull(source).Cast<TItem>()];
    public static List<TNewItem> ToList<TItem, TNewItem>(this IEnumerable<TItem> source, Func<TItem, TNewItem> convertTo)
        => [.. IsNotNull(source).Select(convertTo)];
    public static HashSet<TItem> ToHashSet<TItem>(this IEnumerable source)
        => [.. IsNotNull(source).Cast<TItem>()];
    public static HashSet<TNewItem> ToHashSet<TItem, TNewItem>(this IEnumerable<TItem> source, Func<TItem, TNewItem> convertTo)
        => [.. IsNotNull(source).Select(convertTo)];
    public static Dictionary<TKey, TNewValue> ToDictionary<TKey, TValue, TNewValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Func<TValue, TNewValue> convertToValue)
        where TKey : notnull
        => IsNotNull(source).ToDictionary(i => i.Key, i => convertToValue(i.Value));
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Func<TValue, TValue> convertToValue)
        where TKey : notnull
        => IsNotNull(source).ToDictionary(i => i.Key, i => convertToValue(i.Value));

    #endregion

    #region Indexed

    public static IEnumerable<Indexed<TItem>> AsIndexed<TItem>(this IEnumerable source)
        => source.Cast<TItem>().AsIndexed();
    public static IEnumerable<Indexed<TItem>> AsIndexed<TItem>(this IEnumerable<TItem> source)
        => source.AsIndexed(i => i);
    public static IEnumerable<Indexed<TNewItem>> AsIndexed<TItem, TNewItem>(this IEnumerable<TItem> source, Func<TItem, TNewItem> convertTo)
        => source.Select((v, i) => new Indexed<TNewItem>(i, convertTo(v)));
    public static List<IndexedItem<TItem>> ToIndexedList<TItem>(this IEnumerable source)
        => source.Cast<TItem>().ToIndexedList();
    public static List<IndexedItem<TItem>> ToIndexedList<TItem>(this IEnumerable<TItem> source)
        => source.ToIndexedList(i => i);
    public static List<IndexedItem<TNewItem>> ToIndexedList<TItem, TNewItem>(this IEnumerable<TItem> source, Func<TItem, TNewItem> convertTo) {
        using var enumerator = source.GetEnumerator();
        var list = new List<IndexedItem<TNewItem>>();
        var index = 0;
        var hasNext = enumerator.MoveNext();
        while (hasNext) {
            var value = convertTo(enumerator.Current);
            hasNext = enumerator.MoveNext();
            list.Add(new(index++, value, !hasNext));
        }
        return list;
    }

    #endregion

    #region ForEach

    public static void ForEach<TItem>(this IEnumerable source, Action<TItem> action) {
        using var enumerator = source.Cast<TItem>().GetEnumerator();
        while (enumerator.MoveNext())
            action(enumerator.Current);
    }

    public static void ForEach<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source, Action<TKey, TValue> action)
        where TKey : notnull {
        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
            action(enumerator.Current.Key, enumerator.Current.Value);
    }

#endregion
}
