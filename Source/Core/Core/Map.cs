namespace DotNetToolbox;

public class Map(IEnumerable<KeyValuePair<string, object>>? source = null)
    : Map<object>(source),
      IMap {
    public static Map Empty() => [];
    public static Map<TValue> Empty<TValue>()
        where TValue : notnull
        => [];

    public static Map FromValue(string key, object value)
        => new() { [key] = value };

    public static Map FromMap(IEnumerable<KeyValuePair<string, object>> dict)
        => [.. dict];
    public static Map<TValue> FromMap<TValue>(IEnumerable<KeyValuePair<string, TValue>> dict)
        where TValue : notnull
        => [.. dict];
    public static Map FromList(IEnumerable<object> list, Func<object, string> keySelector)
        => [.. list.ToDictionary(keySelector)];
    public static Map<TValue> FromList<TValue>(IEnumerable<TValue> list, Func<TValue, string> keySelector)
        where TValue : notnull
        => [.. list.ToDictionary(keySelector)];
    public static Map<Map<string>> FromTable(IEnumerable<IEnumerable<string>> table) {
        var rows = table.ToArray(r => r.ToArray());
        if (rows.Length < 2 || rows[0].Length == 0) return Empty<Map<string>>();
        var result = new Map<Map<string>>();
        var headers = new Map<string>();
        foreach (var col in rows[0]) {
            var header = col.Split(':');
            headers[header[0]] = header.Length > 1 ? header[1] : string.Empty;
        }
        result["Header"] = FromMap(headers);
        for (var row = 1; row < rows.Length; row++) {
            var rowMap = new Map<string>();
            for (var col = 0; col < rows[row].Length; col++) {
                var cell = rows[row][col];
                var header = headers.ElementAtOrDefault(col).Key;
                rowMap[header] = cell;
            }
            result[row.ToString()] = rowMap;
        }
        return result;
    }
    public TValue? GetValueAsOrDefault<TValue>(string key, TValue? defaultValue = default)
        => TryGetValueAs<TValue>(key, out var value)
               ? value
               : defaultValue;

    public TValue GetValueAs<TValue>(string key)
        => !TryGetValue(key, out var value)
               ? throw new InvalidOperationException($"The value for key '{key}' was not found.")
               : value switch {
                   TValue result => result,
                   not null when value.GetType().IsAssignableTo(typeof(TValue)) => (TValue)value,
                   _ => throw new InvalidCastException($"The value for key '{key}' cannot be converted to '{typeof(TValue).Name}'."),
               };

    public bool TryGetValueAs<TValue>(string key, [MaybeNullWhen(false)] out TValue value) {
        value = default;
        if (!TryGetValue(key, out var obj)) return false;
        switch (obj) {
            case TValue result:
                value = result;
                return true;
            case not null when obj.GetType().IsAssignableTo(typeof(TValue)):
                value = (TValue)obj;
                return true;
            default: return false;
        }
    }
}

public class Map<TValue>
    : MapBase,
      IMap<TValue> {
    private readonly ConcurrentDictionary<string, TValue> _data = [];

    private readonly HashSet<string> _myKeys = [];
    private bool _isDisposed;

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing) {
        if (_isDisposed) return;
        if (isDisposing)
            foreach (var key in _myKeys) Remove(key);
        _isDisposed = true;
    }

    public Map(IEnumerable<KeyValuePair<string, TValue>>? source = null) {
        _data = source switch {
            Map<TValue> map => map._data,
            not null => new(source),
            _ => _data,
        };
        _myKeys.AddRange(_data.Keys);
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_data).GetEnumerator();
    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() => _data.GetEnumerator();

    public bool IsReadOnly => false;
    public int Count => _data.Count;

    [AllowNull]
    public TValue this[string key] {
        get => _data[key];
        set {
            lock (_data) {
#pragma warning disable IDE0011 // Add braces
                if (value is null) Remove(key);
#pragma warning restore IDE0011 // Add braces
                else {
                    if (_data.TryGetValue(key, out var item) && item is IDisposable) Remove(key);
                    _data[key] = value;
                }
            }
        }
    }

    public TValue? GetValueOrDefault(string key, TValue? defaultValue = default)
        => TryGetValue(key, out var value)
               ? value
               : defaultValue;
    public TValue GetValue(string key)
        => TryGetValue(key, out var value)
               ? value
               : throw new InvalidOperationException($"The value for key '{key}' was not found.");
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
        => _data.TryGetValue(key, out value);

    public ICollection<string> Keys => _data.Keys;
    public ICollection<TValue> Values => _data.Values;
    public void Clear() {
        foreach (var myKey in _myKeys) Remove(myKey);
    }

    public void Add(string key, TValue value)
        => _data.AddOrUpdate(key,
                             k => {
                                 if (!ContainsKey(k)) _myKeys.Add(k);
                                 return this[k] = IsNotNull(value);
                             },
                             (k, _) => this[k] = IsNotNull(value));

    public bool Remove(string key) => Remove(key, true);
    public bool Remove(string key, bool disposeValue) {
        lock (_data) {
            var removed = _data.TryRemove(key, out var value);
            if (!removed || !_myKeys.Contains(key)) return removed;
            _myKeys.Remove(key);
            if (disposeValue && value is IDisposable disposable) disposable.Dispose();
            return removed;
        }
    }

    public bool ContainsKey(string key)
        => _data.ContainsKey(key);

    public void Add(KeyValuePair<string, TValue> item) => Add(item.Key, item.Value);
    bool ICollection<KeyValuePair<string, TValue>>.Contains(KeyValuePair<string, TValue> item)
        => ContainsKey(item.Key)
        && (this[item.Key]?.Equals(item.Value) ?? false);
    bool ICollection<KeyValuePair<string, TValue>>.Remove(KeyValuePair<string, TValue> item)
        => Remove(item.Key);

    protected override void ToText(StringBuilder builder, string? name = null, uint level = 0) {
        if (Keys.Count == 0) return;
        var indent = new string(' ', (int)level * 4);
        if (!string.IsNullOrWhiteSpace(name)) builder.Append($"{name}:");
        if (Keys.Count == 0) builder.AppendLine(" [Empty]");
        builder.AppendLine();
        foreach (var key in Keys) {
            var myKeyMarker = _myKeys.Contains(key) ? "*" : string.Empty;
            builder.Append($"{indent}- {myKeyMarker}");
            BuildItem(builder, key, this[key], level);
        }
    }

    public override string ToString() {
        var builder = new StringBuilder();
        ToText(builder);
        return builder.ToString();
    }

    void ICollection<KeyValuePair<string, TValue>>.CopyTo(KeyValuePair<string, TValue>[] array, int index)
        => _data.ToArray().CopyTo(array, index);
}
