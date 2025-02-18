using static DotNetToolbox.Results.ValidationState;

namespace DotNetToolbox.Results;

public record Result
    : OperationResult<ValidationState>
    , IResult
    , IResultOperations<Result> {
    [SetsRequiredMembers]
    internal Result(IEnumerable<OperationError>? errors = null)
        : base(errors?.Any() ?? false ? Failure : Success, errors ?? []) {
        State = HasErrors ? Failure : Success;
    }

    public static Result AdditiveIdentity => new();

    public bool IsSuccess => Is(Success);
    public bool IsFailure => Is(Failure);

    public static implicit operator Result(string error) => (OperationErrors)error;
    public static implicit operator Result(OperationError error) => (OperationErrors)error;
    public static implicit operator Result(OperationError[] errors) => (OperationErrors)errors;
    public static implicit operator Result(List<OperationError> errors) => (OperationErrors)errors;
    public static implicit operator Result(HashSet<OperationError> errors) => (OperationErrors)errors;
    public static implicit operator Result(OperationErrors errors) => new([.. errors]);
    public static implicit operator OperationError[](Result result) => [.. result.Errors];
    public static implicit operator List<OperationError>(Result result) => [.. result.Errors];
    public static implicit operator HashSet<OperationError>(Result result) => [.. result.Errors];
    public static implicit operator OperationErrors(Result result) => [.. result.Errors];
    public static implicit operator ValidationState(Result result) => result.State;

    public static Result operator +(Result left, IResult? right)
        => right is null ? left : new(left.Errors.Union([.. right.Errors]));
    public static Result operator +(Result left, IEnumerable<OperationError>? right)
        => right is null ? left : new(left.Errors.Union(right));
    public static Result operator +(Result left, OperationError? right)
        => right is null ? left : new(left.Errors.Union([right]));
    public static Result operator +(Result left, string? right)
        => right is null ? left : new(left.Errors.Union([right]));

    public virtual bool Equals(Result? other)
        => other is not null && Errors.SequenceEqual(other.Errors);

    public override int GetHashCode() => Errors.GetHashCode();
}

public record Result<TValue>
    : OperationResult<ValidationState, TValue>
    , IResult<TValue>
    , IResultOperations<Result<TValue>, TValue> {
    [SetsRequiredMembers]
    internal Result(TValue value, IEnumerable<OperationError>? errors = null)
        : base((errors ?? []).Any() ? Failure : Success, value, errors) {
    }

    [SetsRequiredMembers]
    internal Result(IEnumerable<OperationError>? errors = null)
        : this(default!, errors) {
    }

    public bool IsSuccess => Is(Success);
    public bool IsFailure => Is(Failure);

    public static Result<TValue> AdditiveIdentity => new();

    public static implicit operator Result<TValue>(TValue value) => new(value);
    public static implicit operator Result<TValue>(string error) => (OperationErrors)error;
    public static implicit operator Result<TValue>(OperationError error) => (OperationErrors)error;
    public static implicit operator Result<TValue>(OperationError[] error) => (OperationErrors)error;
    public static implicit operator Result<TValue>(List<OperationError> error) => (OperationErrors)error;
    public static implicit operator Result<TValue>(HashSet<OperationError> error) => (OperationErrors)error;
    public static implicit operator Result<TValue>(OperationErrors errors) => new(errors.AsEnumerable());
    public static implicit operator Result<TValue>(Result result) => new(result.Errors);
    public static implicit operator OperationError[](Result<TValue> result) => [.. result.Errors];
    public static implicit operator List<OperationError>(Result<TValue> result) => [.. result.Errors];
    public static implicit operator HashSet<OperationError>(Result<TValue> result) => [.. result.Errors];
    public static implicit operator OperationErrors(Result<TValue> result) => [.. result.Errors];
    public static implicit operator Result(Result<TValue> result) => new(result.Errors);
    public static implicit operator TValue(Result<TValue> result) => result.Value;
    public static implicit operator ValidationState(Result<TValue> result) => result.State;

    public static Result<TValue> operator +(Result<TValue> left, IResult? right)
        => right is null ? left : new(left.Value, left.Errors.Union(right.Errors));
    public static Result<TValue> operator +(Result<TValue> left, IEnumerable<OperationError>? right)
        => right is null ? left : new(left.Value, left.Errors.Union(right));
    public static Result<TValue> operator +(Result<TValue> left, OperationError? right)
        => right is null ? left : new(left.Value, left.Errors.Union([right]));
    public static Result<TValue> operator +(Result<TValue> left, string? right)
        => right is null ? left : new(left.Value, left.Errors.Union([right]));

    public virtual bool Equals(Result<TValue>? other)
        => other is not null && (Value?.Equals(other.Value) ?? other.Value is null) && Errors.SequenceEqual(other.Errors);

    public override int GetHashCode()
        => HashCode.Combine(Value?.GetHashCode() ?? 0, Errors.GetHashCode());
}
