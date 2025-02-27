namespace DotNetToolbox.Results;

public interface IResult
    : IHasErrors {
    bool IsSuccessful { get; }
    bool IsFailure { get; }
    void EnsureIsSuccess();
}

public interface IMergeResult<TSelf>
    where TSelf : IMergeResult<TSelf> {
    static abstract TSelf operator +(TSelf result, Error? error);
    static abstract TSelf operator +(TSelf result, IEnumerable<Error>? errors);
    static abstract TSelf operator +(TSelf result, IResult? other);
    static abstract TSelf operator +(TSelf result, Result? other);
}

public interface IConvertToResult<out TSelf>
    where TSelf : IConvertToResult<TSelf> {
    static abstract implicit operator TSelf(Error error);
    static abstract implicit operator TSelf(Error[]? errors);
    static abstract implicit operator TSelf(List<Error>? errors);
    static abstract implicit operator TSelf(HashSet<Error>? errors);
}

public interface IResult<out TValue>
    : IResult
    , IHasValue<TValue>;
