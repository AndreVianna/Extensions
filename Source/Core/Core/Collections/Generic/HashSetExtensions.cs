namespace System.Collections.Generic;

public static class HashSetExtensions {
    public static void AddRange<TValue>(this HashSet<TValue> source, IEnumerable<TValue> values) {
        foreach (var value in values)
            source.Add(value);
    }
}
