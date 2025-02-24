namespace DotNetToolbox.Results;

public interface IHasValue<out TValue> {
    TValue Value { get; }
}
