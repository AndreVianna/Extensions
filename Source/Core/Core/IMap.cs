namespace DotNetToolbox;

public interface IMap
    : IMap<object> {
    TValue GetValueAs<TValue>(string key);
    TValue? GetValueAsOrDefault<TValue>(string key, TValue? defaultValue = default);
    bool TryGetValueAs<TValue>(string key, [MaybeNullWhen(false)] out TValue value);
}

[SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "Does not apply")]
public interface IMap<TValue>
    : IDictionary<string, TValue>,
      IDisposable {
    bool Remove(string key, bool disposeValue = true);
    TValue? GetValueOrDefault(string key, TValue? defaultValue = default);
    TValue GetValue(string key);
}
