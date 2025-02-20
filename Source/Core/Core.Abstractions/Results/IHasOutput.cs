namespace DotNetToolbox.Results;

public interface IHasOutput<TOutput> {
    TOutput Output { get; }
    bool TryGetValue([NotNullWhen(true)] out TOutput? value);
}
