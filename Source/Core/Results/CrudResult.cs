using static DotNetToolbox.Results.CrudState;

namespace DotNetToolbox.Results;

public record CrudResult
    : OperationResult<CrudState>
    , ICrudResult
    , IResultOperations<CrudResult> {
    [SetsRequiredMembers]
    internal CrudResult(CrudState state = Success, IEnumerable<OperationError>? errors = null)
        : base(state, errors ?? []) {
        if (Errors.Any() && Is(Success)) State = Invalid;
    }

    public bool IsSuccess => Is(Success);
    public bool IsInvalid => Is(Invalid);
    public bool IsNotFound => Is(NotFound);
    public bool IsConflict => Is(Conflict);

    public static CrudResult AdditiveIdentity => new();

    public static implicit operator CrudResult(string error) => (Result)error;
    public static implicit operator CrudResult(OperationError error) => (Result)error;
    public static implicit operator CrudResult(OperationError[] errors) => (Result)errors;
    public static implicit operator CrudResult(List<OperationError> errors) => (Result)errors;
    public static implicit operator CrudResult(HashSet<OperationError> errors) => (Result)errors;
    public static implicit operator CrudResult(OperationErrors errors) => (Result)errors;
    public static implicit operator CrudResult(Result result) => new(Success, result.Errors.AsEnumerable());
    public static implicit operator OperationError[](CrudResult result) => [.. result.Errors];
    public static implicit operator List<OperationError>(CrudResult result) => [.. result.Errors];
    public static implicit operator HashSet<OperationError>(CrudResult result) => [.. result.Errors];
    public static implicit operator OperationErrors(CrudResult result) => [.. result.Errors];
    public static implicit operator Result(CrudResult result) => new(result.Errors);
    public static implicit operator CrudState(CrudResult result) => result.State;

    public static CrudResult operator +(CrudResult left, IResult? right)
        => right is null ? left : new(left.State, left.Errors.Union([.. right.Errors]));
    public static CrudResult operator +(CrudResult left, IEnumerable<OperationError>? right)
        => right is null ? left : new(left.State, [.. left.Errors.Union(right)]);
    public static CrudResult operator +(CrudResult left, OperationError? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));
    public static CrudResult operator +(CrudResult left, string? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));
}

public record CrudResult<TValue>
    : OperationResult<CrudState, TValue>
    , ICrudResult<TValue>
    , IResultOperations<CrudResult<TValue>, TValue> {
    [SetsRequiredMembers]
    internal CrudResult(CrudState state = Success, TValue value = default!, IEnumerable<OperationError>? errors = null)
        : base(state, value, errors ?? []) {
        if (Errors.Any() && Is(Success)) State = Invalid;
    }

    public bool IsSuccess => Is(Success);
    public bool IsInvalid => Is(Invalid);
    public bool IsNotFound => Is(NotFound);
    public bool IsConflict => Is(Conflict);

    public static CrudResult<TValue> AdditiveIdentity => new();

    public static implicit operator CrudResult<TValue>(TValue value) => new(Success, value);
    public static implicit operator CrudResult<TValue>(string error) => (Result<TValue>)error;
    public static implicit operator CrudResult<TValue>(OperationError error) => (Result<TValue>)error;
    public static implicit operator CrudResult<TValue>(OperationErrors errors) => (Result<TValue>)errors;
    public static implicit operator CrudResult<TValue>(OperationError[] errors) => (Result<TValue>)errors;
    public static implicit operator CrudResult<TValue>(List<OperationError> errors) => (Result<TValue>)errors;
    public static implicit operator CrudResult<TValue>(HashSet<OperationError> errors) => (Result<TValue>)errors;
    public static implicit operator CrudResult<TValue>(Result<TValue> result) => new(Success, result.Value, result.Errors);
    public static implicit operator OperationError[](CrudResult<TValue> result) => [.. result.Errors];
    public static implicit operator List<OperationError>(CrudResult<TValue> result) => [.. result.Errors];
    public static implicit operator HashSet<OperationError>(CrudResult<TValue> result) => [.. result.Errors];
    public static implicit operator OperationErrors(CrudResult<TValue> result) => [.. result.Errors];
    public static implicit operator Result<TValue>(CrudResult<TValue> result) => new(result.Value, result.Errors);
    public static implicit operator CrudState(CrudResult<TValue> result) => result.State;
    public static implicit operator TValue(CrudResult<TValue> result) => result.Value;

    public static CrudResult<TValue> operator +(CrudResult<TValue> left, IResult? right)
        => right is null ? left : new(left.State, left.Value, left.Errors.Union([.. right.Errors]));
    public static CrudResult<TValue> operator +(CrudResult<TValue> left, IEnumerable<OperationError>? right)
        => right is null ? left : new(left.State, left.Value, [.. left.Errors.Union(right)]);
    public static CrudResult<TValue> operator +(CrudResult<TValue> left, OperationError? right)
        => right is null ? left : new(left.State, left.Value, left.Errors.Union([right]));
    public static CrudResult<TValue> operator +(CrudResult<TValue> left, string? right)
        => right is null ? left : new(left.State, left.Value, left.Errors.Union([right]));
}
