namespace DotNetToolbox.Results;

public abstract record ResultBase
    : IResultBase {
    protected ResultBase(IEnumerable<Error>? errors = null) {
        Errors = errors as HashSet<Error> ?? errors?.ToHashSet() ?? [];
    }

    /// <summary>
    /// The collection of unique errors. If empty, the Result is considered a Success.
    /// </summary>
    public IReadOnlySet<Error> Errors { get; }

    /// <summary>
    /// True if the result has at least one error.
    /// </summary>
    public bool HasErrors => Errors.Count != 0;

    /// <summary>
    /// A Success result has no errors.
    /// </summary>
    public bool IsSuccessful => !HasErrors;

    /// <summary>
    /// A Failure result has at least one error.
    /// </summary>
    public bool IsFailure => HasErrors;

    public void EnsureIsSuccess() {
        if (IsFailure) throw new OperationFailureException(Errors);
    }
}

public record Result
    : ResultBase
    , IResult
    , IHasDefault<Result>
    , IMergeResult<Result>
    , IConvertToResult<Result> {
    internal Result(IEnumerable<Error>? errors = null)
        : base(errors) {
    }

    public virtual Result<TValue> With<TValue>(TValue value)
        => new(value, Errors);

    public virtual Result<TValue> WithNo<TValue>()
        => new(default!, Errors);

    public virtual bool Equals(Result? other)
        => other?.Errors.SequenceEqual(Errors) ?? false;

    public override int GetHashCode()
        => Errors.Aggregate(0, HashCode.Combine);

    // Factory methods

    /// <summary>
    /// Alias for Success().
    /// </summary>
    public static Result Default => new();

    /// <summary>
    /// Creates a Success result (i.e. with no errors).
    /// </summary>
    public static Result Success() => new();

    /// <summary>
    /// Creates a Failure result with at least one error.
    /// </summary>
    /// <param name="error">The first error (must not be null).</param>
    /// <param name="additionalErrors">Optional additional errors (null errors are ignored).</param>
    /// <returns>A Failure result with a unique set of errors.</returns>
    public static Result Failure(Error error, params IEnumerable<Error> additionalErrors) {
        ArgumentNullException.ThrowIfNull(error);
        return Failure([error, .. additionalErrors]);
    }

    /// <summary>
    /// Creates a Failure result with at least one error.
    /// </summary>
    /// <param name="errors">List of errors (must not be null).</param>
    /// <returns>A Failure result with a unique set of errors.</returns>
    public static Result Failure(IEnumerable<Error> errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(errors);
    }

    /// <summary>
    /// Creates a Success result containing a value.
    /// </summary>
    public static Result<TValue> Success<TValue>(TValue value)
        => new(value);

    /// <summary>
    /// Creates a Failure result. The value is set to default.
    /// </summary>
    public static Result<TValue> Failure<TValue>(TValue value, Error error, params IEnumerable<Error> additionalErrors) {
        ArgumentNullException.ThrowIfNull(error);
        return Failure(value, [error, .. additionalErrors]);
    }

    /// <summary>
    /// Creates a Failure result. The value is set to default.
    /// </summary>
    public static Result<TValue> Failure<TValue>(TValue value, IEnumerable<Error> errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(value, errors);
    }

    // Addition operator overloads

    /// <summary>
    /// Adding a collection of errors.
    /// - If the collection is null or empty, returns the current result.
    /// - Otherwise, adds the errors (ignoring nulls) uniquely.
    /// </summary>
    public static Result operator +(Result result, Error? error) {
        ArgumentNullException.ThrowIfNull(result);
        return error is null
                   ? result
                   : new([.. result.Errors, error]);
    }

    /// <summary>
    /// Adding a collection of errors.
    /// - If the collection is null or empty, returns the current result.
    /// - Otherwise, adds the errors (ignoring nulls) uniquely.
    /// </summary>
    public static Result operator +(Result result, IEnumerable<Error>? errors) {
        ArgumentNullException.ThrowIfNull(result);
        var enumerable = errors as Error[] ?? errors?.ToArray() ?? [];
        return enumerable.Length == 0
                   ? result
                   : new([.. result.Errors, .. enumerable]);
    }

    /// <summary>
    /// Adding a collection of errors.
    /// - If the collection is null or empty, returns the current result.
    /// - Otherwise, adds the errors (ignoring nulls) uniquely.
    /// </summary>
    public static Result operator +(Result result, IResult? other) {
        ArgumentNullException.ThrowIfNull(result);
        return other is null
                   ? result
                   : new([.. result.Errors, .. other.Errors]);
    }

    /// <summary>
    /// Adding another Result.
    /// - If the other Result is null or a Success, returns the current result.
    /// - If the other Result is a Failure:
    ///   - If current result is Success, returns a new Failure that equals the other.
    ///   - If both are Failures, merges the errors (uniquely).
    /// </summary>
    public static Result operator +(Result result, Result? other) {
        ArgumentNullException.ThrowIfNull(result);
        return other is null
                   ? result
                   : new([.. result.Errors, .. other.Errors]);
    }

    // Implicit conversions

    /// <summary>
    /// Implicitly converts an Error to a Result.
    /// </summary>
    public static implicit operator Result(Error error) {
        ArgumentNullException.ThrowIfNull(error);
        return new([error]);
    }

    /// <summary>
    /// Implicitly converts an array of Errors to a Result.
    /// </summary>
    public static implicit operator Result(Error[]? errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(errors.AsEnumerable());
    }

    /// <summary>
    /// Implicitly converts a List of Errors to a Result.
    /// </summary>
    public static implicit operator Result(List<Error>? errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(errors.AsEnumerable());
    }

    /// <summary>
    /// Implicitly converts a HashSet of Errors to a Result.
    /// </summary>
    public static implicit operator Result(HashSet<Error>? errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(errors.AsEnumerable());
    }
}

public record Result<TValue>
    : ResultBase
    , IResult<TValue>
    , IMergeResult<Result<TValue>>
    , IConvertToResult<Result<TValue>> {
    internal Result(TValue value, IEnumerable<Error>? errors = null)
        : base(errors) {
        Value = value;
    }

    /// <summary>
    /// When the Result is a success, this holds the value.
    /// Otherwise, it throws an InvalidOperationException.
    /// </summary>
    public TValue Value { get; }

    // Operator overloads

    /// <summary>
    /// Adds an error.
    /// </summary>
    public static Result<TValue> operator +(Result<TValue> result, Error? error) {
        ArgumentNullException.ThrowIfNull(result);
        return error is null
                   ? result
                   : new(result.Value, [.. result.Errors, error]);
    }

    /// <summary>
    /// Adds a collection of errors.
    /// </summary>
    public static Result<TValue> operator +(Result<TValue> result, IEnumerable<Error>? errors) {
        ArgumentNullException.ThrowIfNull(result);
        var enumerable = errors as Error[] ?? errors?.ToArray() ?? [];
        return enumerable.Length == 0
                   ? result
                   : new(result.Value, [.. result.Errors, .. enumerable]);
    }

    /// <summary>
    /// Merges errors from another Result.
    /// </summary>
    public static Result<TValue> operator +(Result<TValue> result, IResult? other) {
        ArgumentNullException.ThrowIfNull(result);
        return other is null
                   ? result
                   : new(result.Value, [.. result.Errors, .. other.Errors]);
    }

    /// <summary>
    /// Merges errors from another Result.
    /// </summary>
    public static Result<TValue> operator +(Result<TValue> result, Result? other) {
        ArgumentNullException.ThrowIfNull(result);
        return other is null
                   ? result
                   : new(result.Value, [.. result.Errors, .. other.Errors]);
    }

    public virtual bool Equals(Result<TValue>? other)
        => base.Equals(other) && (other.Value?.Equals(Value) ?? Value is null);

    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Value?.GetHashCode() ?? 0);

    // Implicit conversions

    /// <summary>
    /// Implicitly converts a value to a Success Result.
    /// </summary>
    public static implicit operator Result<TValue>(TValue value) => new(value);

    // Implicit conversions

    /// <summary>
    /// Implicitly converts an Error to a Result.
    /// </summary>
    public static implicit operator Result<TValue>(Error error) {
        ArgumentNullException.ThrowIfNull(error);
        return new(default!, [error]);
    }

    /// <summary>
    /// Implicitly converts an array of Errors to a Result.
    /// </summary>
    public static implicit operator Result<TValue>(Error[]? errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(default!, errors.AsEnumerable());
    }

    /// <summary>
    /// Implicitly converts a List of Errors to a Result.
    /// </summary>
    public static implicit operator Result<TValue>(List<Error>? errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(default!, errors.AsEnumerable());
    }

    /// <summary>
    /// Implicitly converts a HashSet of Errors to a Result.
    /// </summary>
    public static implicit operator Result<TValue>(HashSet<Error>? errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(default!, errors.AsEnumerable());
    }

    /// <summary>
    /// Implicitly converts a Result to a Result of Value.
    /// </summary>
    public static implicit operator Result<TValue>(Result? result) {
        ArgumentNullException.ThrowIfNull(result);
        return new(default!, result.Errors);
    }

    /// <summary>
    /// Implicitly converts a HashSet of Errors to a Result.
    /// </summary>
    public static implicit operator Result(Result<TValue>? result) {
        ArgumentNullException.ThrowIfNull(result);
        return new(result.Errors);
    }
}
