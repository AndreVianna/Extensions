namespace DotNetToolbox.Results;

public interface ICrudResult
    : IHasErrors {
    bool IsSuccess { get; } // The operation was successful.
    bool IsInvalid { get; } // The input record is invalid.
    bool IsNotFound { get; } // The input record was not found.
    bool IsConflict { get; } // A conflict has occured blocking the operation.
    ICrudResult<TOutput> SetOutput<TOutput>(TOutput output);
    ICrudResult MergeWith(IValidationResult other);
}

public interface ICrudResult<TValue>
    : ICrudResult, IHasOutput<TValue> {
    new ICrudResult<TValue> MergeWith(IValidationResult other);
}
