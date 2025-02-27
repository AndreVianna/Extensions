namespace DotNetToolbox.Results;

public static class TypedResult {
    /// <summary>
    /// Creates a typed result with at least one error.
    /// </summary>
    /// <param name="status">The status of the result. </param>
    /// <param name="errors">Optional errors (null errors are ignored).</param>
    /// <returns>A typed result with a unique set of errors.</returns>
    public static TypedResult<TStatus> As<TStatus>(TStatus status, params IEnumerable<Error> errors)
        where TStatus : Enum
        => new(status, errors);

    /// <summary>
    /// Creates a typed result with at least one error.
    /// </summary>
    /// <param name="status">The status of the result. </param>
    /// <param name="value">The value of the result. </param>
    /// <param name="errors">Optional errors (null errors are ignored).</param>
    /// <returns>A typed result with a unique set of errors.</returns>
    public static TypedResult<TStatus, TValue> As<TStatus, TValue>(TStatus status, TValue value, params IEnumerable<Error> errors)
        where TStatus : Enum
        => new(status, value, errors);
}

public abstract record TypedResultBase<TStatus>
    : ResultBase
    , ITypedResultBase<TStatus>
    where TStatus : Enum {
    protected TypedResultBase(TStatus status, IEnumerable<Error>? errors = null)
        : base(errors) {
        Status = status;
    }

    public TStatus Status { get; }

    /// <summary>
    /// Check if the result is of a specific status.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <returns>True if the result is of the specified status.</returns>
    public bool Is(TStatus status) => Status.Equals(status);

    public void EnsureIs(TStatus status) {
        if (!Is(status)) throw new OperationFailureException(Errors);
    }
}

public record TypedResult<TStatus>
    : TypedResultBase<TStatus>
    , IMergeResult<TypedResult<TStatus>>
    , ITypedResult<TStatus>
    where TStatus : Enum {
    internal TypedResult(TStatus status, IEnumerable<Error>? errors = null)
        : base(status, errors) {
    }

    public virtual TypedResult<TStatus, TValue> With<TValue>(TValue value)
        => new(Status, value, Errors);

    public virtual TypedResult<TStatus, TValue> WithNo<TValue>()
        => new(Status, default!, Errors);

    public virtual bool Equals(TypedResult<TStatus>? other)
        => base.Equals(other) && other.Is(Status);
    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Status.GetHashCode());

    // Addition operator overloads

    /// <summary>
    /// Adding a single error.
    /// - If error is null, returns the current result.
    /// - If current result is Success, becomes typed with the error.
    /// - If already Failure, the error is added (uniquely).
    /// </summary>
    public static TypedResult<TStatus> operator +(TypedResult<TStatus> result, Error? error) {
        ArgumentNullException.ThrowIfNull(result);
        return error is null
                   ? result
                   : new(result.Status, [.. result.Errors, error]);
    }

    /// <summary>
    /// Adding a collection of errors.
    /// - If the collection is null or empty, returns the current result.
    /// - Otherwise, adds the errors (ignoring nulls) uniquely.
    /// </summary>
    public static TypedResult<TStatus> operator +(TypedResult<TStatus> result, IEnumerable<Error>? errors) {
        ArgumentNullException.ThrowIfNull(result);
        var enumerable = errors as Error[] ?? errors?.ToArray() ?? [];
        return enumerable.Length == 0
                   ? result
                   : new(result.Status, [.. result.Errors, .. enumerable]);
    }

    /// <summary>
    /// Adding another Result.
    /// - If the other Result is null or a Success, returns the current result.
    /// - If the other Result is a Failure:
    ///   - If current result is Success, returns a new typed that equals the other.
    ///   - If both are Failures, merges the errors (uniquely).
    /// </summary>
    public static TypedResult<TStatus> operator +(TypedResult<TStatus> result, IResult? other) {
        ArgumentNullException.ThrowIfNull(result);
        return other is null
                   ? result
                   : new(result.Status, [.. result.Errors, .. other.Errors]);
    }

    /// <summary>
    /// Adding a collection of errors.
    /// - If the collection is null or empty, returns the current result.
    /// - Otherwise, adds the errors (ignoring nulls) uniquely.
    /// </summary>
    public static TypedResult<TStatus> operator +(TypedResult<TStatus> result, Result? other) {
        ArgumentNullException.ThrowIfNull(result);
        return other is null
                   ? result
                   : new(result.Status, [.. result.Errors, .. other.Errors]);
    }
}

public sealed record TypedResult<TStatus, TValue>
    : TypedResultBase<TStatus>
    , IMergeResult<TypedResult<TStatus, TValue>>
    , ITypedResult<TStatus, TValue>
    where TStatus : Enum {
    internal TypedResult(TStatus status, TValue value, IEnumerable<Error>? errors = null)
        : base(status, errors) {
        Value = value;
    }

    /// <summary>
    /// When the Result is a success, this holds the value.
    /// Otherwise, it throws an InvalidOperationException.
    /// </summary>
    public TValue Value { get; }

    // Addition operator overloads

    /// <summary>
    /// Adds a single error.
    /// </summary>
    public static TypedResult<TStatus, TValue> operator +(TypedResult<TStatus, TValue> result, Error? error) {
        ArgumentNullException.ThrowIfNull(result);
        return error is null
                   ? result
                   : new(result.Status, result.Value, [.. result.Errors, error]);
    }

    /// <summary>
    /// Adds a collection of errors.
    /// </summary>
    public static TypedResult<TStatus, TValue> operator +(TypedResult<TStatus, TValue> result, IEnumerable<Error>? errors) {
        ArgumentNullException.ThrowIfNull(result);
        var enumerable = errors as Error[] ?? errors?.ToArray() ?? [];
        return enumerable.Length == 0
                   ? result
                   : new(result.Status, result.Value, [.. result.Errors, .. enumerable]);
    }

    /// <summary>
    /// Merges errors from another Result.
    /// </summary>
    public static TypedResult<TStatus, TValue> operator +(TypedResult<TStatus, TValue> result, IResult? other) {
        ArgumentNullException.ThrowIfNull(result);
        return other is null
                   ? result
                   : new(result.Status, result.Value, [.. result.Errors, .. other.Errors]);
    }

    /// <summary>
    /// Merges errors from another Result.
    /// </summary>
    public static TypedResult<TStatus, TValue> operator +(TypedResult<TStatus, TValue> result, Result? other) {
        ArgumentNullException.ThrowIfNull(result);
        return other is null
                   ? result
                   : new(result.Status, result.Value, [.. result.Errors, .. other.Errors]);
    }

    public bool Equals(TypedResult<TStatus, TValue>? other)
        => base.Equals(other) && other.Is(Status);

    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Status.GetHashCode());

    /// <summary>
    /// Implicitly converts a HashSet of Errors to a Result.
    /// </summary>
    public static implicit operator TypedResult<TStatus>(TypedResult<TStatus, TValue>? result) {
        ArgumentNullException.ThrowIfNull(result);
        return new(result.Status, result.Errors);
    }
}
