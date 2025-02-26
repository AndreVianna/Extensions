namespace DotNetToolbox.Results;

public interface IResult
    : IHasErrors {
    bool IsSuccessful { get; }
    bool IsFailure { get; }
    void EnsureIsSuccess();
}

public interface IResult<out TValue>
    : IResult
    , IHasValue<TValue>;
