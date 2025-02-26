using DotNetToolbox.Singletons;

namespace DotNetToolbox.Results;

public record Result
    : IResult
    , IHasDefault<Result>
    , IResultFactory {
    internal Result(IEnumerable<IError>? errors = null) {
        Errors = errors as HashSet<IError> ?? errors?.ToHashSet() ?? [];
    }

    /// <summary>
    /// The collection of unique errors. If empty, the Result is considered a Success.
    /// </summary>
    public IReadOnlySet<IError> Errors { get; }

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
    public static Result Failure(IError error, params IEnumerable<IError> additionalErrors) {
        ArgumentNullException.ThrowIfNull(error);
        return Failure([error, .. additionalErrors]);
    }

    /// <summary>
    /// Creates a Failure result with at least one error.
    /// </summary>
    /// <param name="error">The first error (must not be null).</param>
    /// <param name="additionalErrors">Optional additional errors (null errors are ignored).</param>
    /// <returns>A Failure result with a unique set of errors.</returns>
    public static Result Failure(Error error, params IEnumerable<IError> additionalErrors)
        => Failure((IError)error, additionalErrors);

    /// <summary>
    /// Creates a Failure result with at least one error.
    /// </summary>
    /// <param name="errors">List of errors (must not be null).</param>
    /// <returns>A Failure result with a unique set of errors.</returns>
    public static Result Failure(IEnumerable<IError> errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(errors);
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
    public static Result<TValue> Failure<TValue>(TValue value, IError error, params IEnumerable<IError> additionalErrors) {
        ArgumentNullException.ThrowIfNull(error);
        return Failure(value, [error, .. additionalErrors]);
    }

    /// <summary>
    /// Creates a Failure result. The value is set to default.
    /// </summary>
    public static Result<TValue> Failure<TValue>(TValue value, Error error, params IEnumerable<IError> additionalErrors) {
        ArgumentNullException.ThrowIfNull(error);
        return Failure(value, [error, .. additionalErrors]);
    }

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
    public static Result<TValue> Failure<TValue>(TValue value, IError error, params IEnumerable<Error> additionalErrors) {
        ArgumentNullException.ThrowIfNull(error);
        return Failure(value, [error, .. additionalErrors]);
    }

    /// <summary>
    /// Creates a Failure result. The value is set to default.
    /// </summary>
    public static Result<TValue> Failure<TValue>(TValue value, IEnumerable<IError> errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(value, errors);
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
    public static Result operator +(Result result, IEnumerable<IError>? errors) {
        ArgumentNullException.ThrowIfNull(result);
        var enumerable = errors as IError[] ?? errors?.ToArray() ?? [];
        return enumerable.Length == 0
                   ? result
                   : new([.. result.Errors, .. enumerable]);
    }

    /// <summary>
    /// Adding another Result.
    /// - If the other Result is null or a Success, returns the current result.
    /// - If the other Result is a Failure:
    ///   - If current result is Success, returns a new Failure that equals the other.
    ///   - If both are Failures, merges the errors (uniquely).
    /// </summary>
    public static Result operator +(Result result, Result other) {
        ArgumentNullException.ThrowIfNull(result);
        return new([..result.Errors, ..other.Errors]);
    }

    // Implicit conversions

    /// <summary>
    /// Implicitly converts an Error to a Result.
    /// </summary>
    public static implicit operator Result(Error error) => new([error]);

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
    : Result
    , IResult<TValue> {
    internal Result(TValue value, IEnumerable<IError>? errors = null)
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
    /// Adds a collection of errors.
    /// </summary>
    public static Result<TValue> operator +(Result<TValue> result, IEnumerable<IError>? errors) {
        ArgumentNullException.ThrowIfNull(result);
        var enumerable = errors as Error[] ?? errors?.ToArray() ?? [];
        return enumerable.Length == 0
                   ? result
                   : new(result.Value, [.. result.Errors, .. enumerable]);
    }

    /// <summary>
    /// Merges errors from another Result.
    /// </summary>
    public static Result<TValue> operator +(Result<TValue> result, Result other) {
        ArgumentNullException.ThrowIfNull(result);
        return new(result.Value, [.. result.Errors, .. other.Errors]);
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

    /// <summary>
    /// Implicitly converts an Error to a Result.
    /// </summary>
    public static implicit operator Result<TValue>(Error error) => new(default!, [error]);

    /// <summary>
    /// Implicitly converts an array of Errors to a Result.
    /// </summary>
    public static implicit operator Result<TValue>(Error[]? errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(default!, errors);
    }

    /// <summary>
    /// Implicitly converts a List of Errors to a Result.
    /// </summary>
    public static implicit operator Result<TValue>(List<Error>? errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(default!, errors);
    }

    /// <summary>
    /// Implicitly converts a HashSet of Errors to a Result.
    /// </summary>
    public static implicit operator Result<TValue>(HashSet<Error>? errors) {
        ArgumentNullException.ThrowIfNull(errors);
        return new(default!, errors);
    }
}
