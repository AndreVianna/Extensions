using static DotNetToolbox.Results.HttpRequestState;

namespace DotNetToolbox.Results;

public record HttpRequestResult
    : OperationResult<HttpRequestState>
    , IHttpResult
    , IResultOperations<HttpRequestResult> {
    [SetsRequiredMembers]
    internal HttpRequestResult(HttpRequestState state = Ok, IEnumerable<OperationError>? errors = null)
        : base(state, errors ?? []) {
        if (Errors.Any() && (Is(Ok) || Is(Created))) State = BadRequest;
    }

    public bool IsOk => Is(Ok);
    public bool IsCreated => Is(Created);
    public bool IsBadRequest => Is(BadRequest);
    public bool IsUnauthorized => Is(Unauthorized);
    public bool IsNotFound => Is(NotFound);
    public bool IsConflict => Is(Conflict);
    public bool IsServerError => Is(ServerError);

    public static HttpRequestResult AdditiveIdentity => new();

    public static implicit operator HttpRequestResult(string error) => (Result)error;
    public static implicit operator HttpRequestResult(OperationError error) => (Result)error;
    public static implicit operator HttpRequestResult(OperationError[] errors) => (Result)errors;
    public static implicit operator HttpRequestResult(List<OperationError> errors) => (Result)errors;
    public static implicit operator HttpRequestResult(HashSet<OperationError> errors) => (Result)errors;
    public static implicit operator HttpRequestResult(OperationErrors errors) => (Result)errors;
    public static implicit operator HttpRequestResult(Result result) => new(Ok, result.Errors.AsEnumerable());
    public static implicit operator OperationError[](HttpRequestResult result) => [.. result.Errors];
    public static implicit operator List<OperationError>(HttpRequestResult result) => [.. result.Errors];
    public static implicit operator HashSet<OperationError>(HttpRequestResult result) => [.. result.Errors];
    public static implicit operator OperationErrors(HttpRequestResult result) => [.. result.Errors];
    public static implicit operator Result(HttpRequestResult result) => new(result.Errors);
    public static implicit operator HttpRequestState(HttpRequestResult result) => result.State;

    public static HttpRequestResult operator +(HttpRequestResult left, IResult? right)
        => right is null ? left : new(left.State, left.Errors.Union([.. right.Errors]));
    public static HttpRequestResult operator +(HttpRequestResult left, IEnumerable<OperationError>? right)
        => right is null ? left : new(left.State, [.. left.Errors.Union(right)]);
    public static HttpRequestResult operator +(HttpRequestResult left, OperationError? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));
    public static HttpRequestResult operator +(HttpRequestResult left, string? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));
}

public record HttpResult<TValue>
    : OperationResult<HttpRequestState, TValue>
    , IHttpResult<TValue>
    , IResultOperations<HttpResult<TValue>, TValue> {
    [SetsRequiredMembers]
    internal HttpResult(HttpRequestState state = Ok, TValue value = default!, IEnumerable<OperationError>? errors = null)
        : base(state, value, errors ?? []) {
        if (Errors.Any() && (Is(Ok) || Is(Created))) State = BadRequest;
    }

    public bool IsOk => Is(Ok);
    public bool IsCreated => Is(Created);
    public bool IsBadRequest => Is(BadRequest);
    public bool IsUnauthorized => Is(Unauthorized);
    public bool IsNotFound => Is(NotFound);
    public bool IsConflict => Is(Conflict);
    public bool IsServerError => Is(ServerError);

    public static HttpResult<TValue> AdditiveIdentity => new();

    public static implicit operator HttpResult<TValue>(TValue value) => new(Ok, value);
    public static implicit operator HttpResult<TValue>(string error) => (Result<TValue>)error;
    public static implicit operator HttpResult<TValue>(OperationError error) => (Result<TValue>)error;
    public static implicit operator HttpResult<TValue>(OperationErrors errors) => (Result<TValue>)errors;
    public static implicit operator HttpResult<TValue>(OperationError[] errors) => (Result<TValue>)errors;
    public static implicit operator HttpResult<TValue>(List<OperationError> errors) => (Result<TValue>)errors;
    public static implicit operator HttpResult<TValue>(HashSet<OperationError> errors) => (Result<TValue>)errors;
    public static implicit operator HttpResult<TValue>(Result<TValue> result) => new(Ok, result.Value, result.Errors);
    public static implicit operator OperationError[](HttpResult<TValue> result) => [.. result.Errors];
    public static implicit operator List<OperationError>(HttpResult<TValue> result) => [.. result.Errors];
    public static implicit operator HashSet<OperationError>(HttpResult<TValue> result) => [.. result.Errors];
    public static implicit operator OperationErrors(HttpResult<TValue> result) => [.. result.Errors];
    public static implicit operator Result<TValue>(HttpResult<TValue> result) => new(result.Value, result.Errors);
    public static implicit operator TValue(HttpResult<TValue> result) => result.Value;
    public static implicit operator HttpRequestState(HttpResult<TValue> result) => result.State;

    public static HttpResult<TValue> operator +(HttpResult<TValue> left, IResult? right)
        => right is null ? left : new(left.State, left.Value, left.Errors.Union([.. right.Errors]));
    public static HttpResult<TValue> operator +(HttpResult<TValue> left, IEnumerable<OperationError>? right)
        => right is null ? left : new(left.State, left.Value, [.. left.Errors.Union(right)]);
    public static HttpResult<TValue> operator +(HttpResult<TValue> left, OperationError? right)
        => right is null ? left : new(left.State, left.Value, left.Errors.Union([right]));
    public static HttpResult<TValue> operator +(HttpResult<TValue> left, string? right)
        => right is null ? left : new(left.State, left.Value, left.Errors.Union([right]));
}
