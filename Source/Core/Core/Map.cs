﻿namespace DotNetToolbox;

public class Map(IEnumerable<KeyValuePair<string, object>>? source = null)
    : Map<object>(source),
      IMap {
    public TValue? GetValueOrDefaultAs<TValue>(string key)
        => TryGetValueAs<TValue>(key, out var value)
            ? value
            : default;

    public TValue GetValueAs<TValue>(string key)
        => TryGetValueAs<TValue>(key, out var value)
               ? value
               : throw new InvalidCastException("The value does not exist or does not match the requested type.");

    public bool TryGetValueAs<TValue>(string key, [MaybeNullWhen(false)] out TValue value) {
        value = default;
        if (!TryGetValue(key, out var obj)) return false;
        switch (obj) {
            case TValue result:
                value = result;
                return true;
            case not null when obj.GetType()
                                  .IsAssignableTo(typeof(TValue)):
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

    public Map(IEnumerable<KeyValuePair<string, TValue>>? source = null) {
        _data = source switch {
            Map<TValue> map => map._data,
            not null => new(source),
            _ => _data,
        };
        MyKeys.AddRange(_data.Keys);
    }

    protected HashSet<string> MyKeys { get; } = [];

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_data).GetEnumerator();
    public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator() => _data.GetEnumerator();

    public bool IsReadOnly => false;
    public int Count => _data.Count;

    [AllowNull]
    public TValue this[string key] {
        get => _data[key];
        set {
            lock (_data) {
                if (value is null) Remove(key);
                else _data[key] = value;
            }
        }
    }

    public TValue? GetValueOrDefault(string key)
        => TryGetValue(key, out var value)
               ? value
               : default;

    public ICollection<string> Keys => _data.Keys;
    public ICollection<TValue> Values => _data.Values;
    public void Clear() {
        foreach (var myKey in MyKeys) Remove(myKey);
    }

    public void Add(string key, TValue value)
        => _data.AddOrUpdate(key,
                             k => {
                                 if (!ContainsKey(k)) MyKeys.Add(k);
                                 return this[k] = IsNotNull(value);
                             },
                             (k, _) => this[k] = IsNotNull(value));

    public bool Remove(string key) {
        lock (_data) {
            var removed = _data.TryRemove(key, out var value);
            if (!removed || !MyKeys.Contains(key)) return removed;
            MyKeys.Remove(key);
            if (this is IContext && value is IDisposable disposable) disposable.Dispose();
            return removed;
        }
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out TValue value)
        => _data.TryGetValue(key, out value);
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
            var myKeyMarker = MyKeys.Contains(key) ? "*" : string.Empty;
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
