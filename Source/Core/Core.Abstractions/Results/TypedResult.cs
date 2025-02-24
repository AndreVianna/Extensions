namespace DotNetToolbox.Results;

public record TypedResult<TStatus>
    : Result
    , ITypedResult<TStatus>
    , ITypedResultFactory<TStatus>
    where TStatus : Enum {
    internal TypedResult(TStatus status, IEnumerable<IError>? errors = null)
        : base(errors) {
        Status = status;
    }

    protected TStatus Status { get; }

    /// <summary>
    /// Check if the result is of a specific status.
    /// </summary>
    /// <param name="status">The status to check.</param>
    /// <returns>True if the result is of the specified status.</returns>
    public bool Is(TStatus status) => Status.Equals(status);

    public void EnsureIs(TStatus status) {
        if (!Is(status)) throw new ResultException(Errors);
    }

    public virtual bool Equals(TypedResult<TStatus>? other)
        => base.Equals(other) && other.Is(Status);
    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Status.GetHashCode());

    // Factory methods

    /// <summary>
    /// Creates a Failure result with at least one error.
    /// </summary>
    /// <param name="status">The status of the result. </param>
    /// <param name="errors">Optional errors (null errors are ignored).</param>
    /// <returns>A Failure result with a unique set of errors.</returns>
    public static TypedResult<TStatus> As(TStatus status, params IEnumerable<IError> errors)
        => new(status, errors);

    /// <summary>
    /// Creates a Failure result with at least one error.
    /// </summary>
    /// <param name="status">The status of the result. </param>
    /// <param name="value">The value of the result. </param>
    /// <param name="errors">Optional errors (null errors are ignored).</param>
    /// <returns>A Failure result with a unique set of errors.</returns>
    public static TypedResult<TStatus, TValue> As<TValue>(TStatus status, TValue value, params IEnumerable<IError> errors)
        => new(status, value, errors);

    // Addition operator overloads

    /// <summary>
    /// Adding a single error.
    /// - If error is null, returns the current result.
    /// - If current result is Success, becomes Failure with the error.
    /// - If already Failure, the error is added (uniquely).
    /// </summary>
    public static TypedResult<TStatus> operator +(TypedResult<TStatus> result, IError error) {
        ArgumentNullException.ThrowIfNull(result);
        return result + [error];
    }

    /// <summary>
    /// Adding a collection of errors.
    /// - If the collection is null or empty, returns the current result.
    /// - Otherwise, adds the errors (ignoring nulls) uniquely.
    /// </summary>
    public static TypedResult<TStatus> operator +(TypedResult<TStatus> result, IEnumerable<IError>? errors) {
        ArgumentNullException.ThrowIfNull(result);
        var enumerable = errors as IError[] ?? errors?.ToArray() ?? [];
        return enumerable.Length == 0
                   ? result
                   : new(result.Status, [.. result.Errors, .. enumerable]);
    }

    /// <summary>
    /// Adding another Result.
    /// - If the other Result is null or a Success, returns the current result.
    /// - If the other Result is a Failure:
    ///   - If current result is Success, returns a new Failure that equals the other.
    ///   - If both are Failures, merges the errors (uniquely).
    /// </summary>
    public static TypedResult<TStatus> operator +(TypedResult<TStatus> result, IResult other) {
        ArgumentNullException.ThrowIfNull(result);
        return result + other.Errors;
    }
}

public sealed record TypedResult<TStatus, TValue>
    : TypedResult<TStatus>
    , ITypedResult<TStatus, TValue> where TStatus : Enum {
    internal TypedResult(TStatus status, TValue value, IEnumerable<IError>? errors = null)
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
    public static ITypedResult<TStatus, TValue> operator +(TypedResult<TStatus, TValue> result, IError error) {
        ArgumentNullException.ThrowIfNull(result);
        return result + [error];
    }

    /// <summary>
    /// Adds a collection of errors.
    /// </summary>
    public static ITypedResult<TStatus, TValue> operator +(TypedResult<TStatus, TValue> result, IEnumerable<IError>? errors) {
        ArgumentNullException.ThrowIfNull(result);
        var enumerable = errors as Error[] ?? errors?.ToArray() ?? [];
        return enumerable.Length == 0
                   ? result
                   : new(result.Status, result.Value, [.. result.Errors, .. enumerable]);
    }

    /// <summary>
    /// Merges errors from another Result.
    /// </summary>
    public static ITypedResult<TStatus, TValue> operator +(TypedResult<TStatus, TValue> result, Result other) {
        ArgumentNullException.ThrowIfNull(result);
        return result + other.Errors;
    }

    public bool Equals(TypedResult<TStatus, TValue>? other)
        => base.Equals(other) && other.Is(Status);

    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Status.GetHashCode());
}
