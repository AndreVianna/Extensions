namespace DotNetToolbox.Results;

public interface IReturnsValue<TValue> {
    TValue Value { get; }
    bool TryGetValue([NotNullWhen(true)] out TValue value);
}