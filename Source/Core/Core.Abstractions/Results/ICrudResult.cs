namespace DotNetToolbox.Results;

public interface ICrudResult
    : IHasErrors {
    bool IsSuccess { get; } // The operation was successful.
    bool IsInvalid { get; } // The request validation failed.
    bool IsNotFound { get; } // The requested resource was not found.
    bool IsConflict { get; } // A conflict has occured blocking the operation.
}

public interface ICrudResult<TValue>
    : ICrudResult
    , IReturnsValue<TValue>;
