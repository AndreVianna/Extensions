namespace DotNetToolbox.Results;

public interface IResult
    : IHasErrors {
    bool IsSuccess { get; }
    bool IsFailure { get; }
};

public interface IResult<TValue>
    : IResult
    , IReturnsValue<TValue>;
