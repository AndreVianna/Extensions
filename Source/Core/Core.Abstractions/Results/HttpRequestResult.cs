namespace DotNetToolbox.Results;

public sealed record HttpRequestResult
    : Result<HttpRequestState>
    , IHttpRequestResult
    , IResultOperators<HttpRequestResult> {
    [SetsRequiredMembers]
    private HttpRequestResult(HttpRequestState state, IEnumerable<ResultError>? errors = null)
        : base(state, errors ?? []) {
        if (HasErrors) State = HttpRequestState.BadRequest;
    }

    public bool IsOk => Is(HttpRequestState.Ok);
    public bool IsCreated => Is(HttpRequestState.Created);
    public bool IsBadRequest => Is(HttpRequestState.BadRequest);
    public bool IsUnauthorized => Is(HttpRequestState.Unauthorized);
    public bool IsNotFound => Is(HttpRequestState.NotFound);
    public bool IsConflict => Is(HttpRequestState.Conflict);
    public bool IsServerError => Is(HttpRequestState.ServerError);

    public IHttpRequestResult<TOutput> SetOutput<TOutput>(TOutput output) => new HttpRequestResult<TOutput>(State, output, Errors);
    public IHttpRequestResult MergeWith(IValidationResult other) {
        foreach (var error in other.Errors) Errors.Add(error);
        return this;
    }

    public static IHttpRequestResult Default => new HttpRequestResult(HttpRequestState.Ok);

    public static IHttpRequestResult Ok() => new HttpRequestResult(HttpRequestState.Ok);
    public static IHttpRequestResult<TOutput> Ok<TOutput>(TOutput output)
        =>  new HttpRequestResult<TOutput>(HttpRequestState.Ok, output);
    public static IHttpRequestResult Created()
        => new HttpRequestResult(HttpRequestState.Created);
    public static IHttpRequestResult<TOutput> Created<TOutput>(TOutput output)
        => new HttpRequestResult<TOutput>(HttpRequestState.Created, output);
    public static IHttpRequestResult BadRequest(params IEnumerable<ResultError> errors)
        => new HttpRequestResult(HttpRequestState.BadRequest, errors);
    public static IHttpRequestResult BadRequest(string message, string? source = null)
        => BadRequest(new ResultError(message, source ?? string.Empty));
    public static IHttpRequestResult Unauthorized()
        => new HttpRequestResult(HttpRequestState.Unauthorized);
    public static IHttpRequestResult NotFound(params IEnumerable<ResultError> errors)
        => new HttpRequestResult(HttpRequestState.NotFound, errors);
    public static IHttpRequestResult NotFound(string message, string? source = null)
        => BadRequest(new ResultError(message, source ?? string.Empty));
    public static IHttpRequestResult Conflict(params IEnumerable<ResultError> errors)
        => new HttpRequestResult(HttpRequestState.Conflict, errors);
    public static IHttpRequestResult Conflict(string message, string? source = null)
        => Conflict(new ResultError(message, source ?? string.Empty));
    public static IHttpRequestResult Conflict<TOutput>(TOutput output, params IEnumerable<ResultError> errors)
        => new HttpRequestResult<TOutput>(HttpRequestState.Conflict, output, errors);
    public static IHttpRequestResult Conflict<TOutput>(TOutput output, string message, string? source = null)
        => Conflict(output, new ResultError(message, source ?? string.Empty));
    public static IHttpRequestResult ServerError(params IEnumerable<ResultError> errors)
        => new HttpRequestResult(HttpRequestState.ServerError, errors);
    public static IHttpRequestResult ServerError(string message, string? source = null)
        => ServerError(new ResultError(message, source ?? string.Empty));

    public static implicit operator HttpRequestResult(string error) => (ValidationResult)error;
    public static implicit operator HttpRequestResult(ResultError error) => (ValidationResult)error;
    public static implicit operator HttpRequestResult(ResultError[] errors) => (ValidationResult)errors;
    public static implicit operator HttpRequestResult(List<ResultError> errors) => (ValidationResult)errors;
    public static implicit operator HttpRequestResult(HashSet<ResultError> errors) => (ValidationResult)errors;
    public static implicit operator HttpRequestResult(ResultErrors errors) => (ValidationResult)errors;
    public static implicit operator HttpRequestResult(ValidationResult result) => new(HttpRequestState.Ok, result.Errors.AsEnumerable());
    public static implicit operator ResultError[](HttpRequestResult result) => [.. result.Errors];
    public static implicit operator List<ResultError>(HttpRequestResult result) => [.. result.Errors];
    public static implicit operator HashSet<ResultError>(HttpRequestResult result) => [.. result.Errors];
    public static implicit operator ResultErrors(HttpRequestResult result) => [.. result.Errors];
    public static implicit operator ValidationResult(HttpRequestResult result) => new(result.Errors);
    public static implicit operator HttpRequestState(HttpRequestResult result) => result.State;

    public static HttpRequestResult operator +(HttpRequestResult left, IValidationResult? right)
        => right is null ? left : new(left.State, left.Errors.Union([.. right.Errors]));
    public static HttpRequestResult operator +(HttpRequestResult left, IEnumerable<ResultError>? right)
        => right is null ? left : new(left.State, [.. left.Errors.Union(right)]);
    public static HttpRequestResult operator +(HttpRequestResult left, ResultError? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));
    public static HttpRequestResult operator +(HttpRequestResult left, string? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));

    #pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    public bool Equals(HttpRequestResult? other)
        => other is not null && State == other.State && Errors.SequenceEqual(other.Errors);
    #pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.
}

public sealed record HttpRequestResult<TOutput>
    : Result<HttpRequestState, TOutput>
    , IHttpRequestResult<TOutput>
    , IResultOperators<HttpRequestResult<TOutput>, TOutput> {
    [SetsRequiredMembers]
    internal HttpRequestResult(HttpRequestState state = HttpRequestState.Ok, TOutput output = default!, IEnumerable<ResultError>? errors = null)
        : base(state, output, errors ?? []) {
        if (HasErrors) State = HttpRequestState.BadRequest;
    }

    public bool IsOk => Is(HttpRequestState.Ok);
    public bool IsCreated => Is(HttpRequestState.Created);
    public bool IsBadRequest => Is(HttpRequestState.BadRequest);
    public bool IsUnauthorized => Is(HttpRequestState.Unauthorized);
    public bool IsNotFound => Is(HttpRequestState.NotFound);
    public bool IsConflict => Is(HttpRequestState.Conflict);
    public bool IsServerError => Is(HttpRequestState.ServerError);

    IHttpRequestResult<T> IHttpRequestResult.SetOutput<T>(T output) => throw new NotSupportedException();
    IHttpRequestResult IHttpRequestResult.MergeWith(IValidationResult other) => MergeWith(other);
    public IHttpRequestResult<TOutput> MergeWith(IValidationResult other) {
        foreach (var error in other.Errors) Errors.Add(error);
        return this;
    }

    public static implicit operator HttpRequestResult<TOutput>(TOutput output) => new(HttpRequestState.Ok, output);
    public static implicit operator HttpRequestResult<TOutput>(string error) => (ValidationResult<TOutput>)error;
    public static implicit operator HttpRequestResult<TOutput>(ResultError error) => (ValidationResult<TOutput>)error;
    public static implicit operator HttpRequestResult<TOutput>(ResultErrors errors) => (ValidationResult<TOutput>)errors;
    public static implicit operator HttpRequestResult<TOutput>(ResultError[] errors) => (ValidationResult<TOutput>)errors;
    public static implicit operator HttpRequestResult<TOutput>(List<ResultError> errors) => (ValidationResult<TOutput>)errors;
    public static implicit operator HttpRequestResult<TOutput>(HashSet<ResultError> errors) => (ValidationResult<TOutput>)errors;
    public static implicit operator HttpRequestResult<TOutput>(ValidationResult<TOutput> result) => new(HttpRequestState.Ok, result.Output, result.Errors);
    public static implicit operator ResultError[](HttpRequestResult<TOutput> result) => [.. result.Errors];
    public static implicit operator List<ResultError>(HttpRequestResult<TOutput> result) => [.. result.Errors];
    public static implicit operator HashSet<ResultError>(HttpRequestResult<TOutput> result) => [.. result.Errors];
    public static implicit operator ResultErrors(HttpRequestResult<TOutput> result) => [.. result.Errors];
    public static implicit operator ValidationResult(HttpRequestResult<TOutput> result) => new(result.Errors);
    public static implicit operator ValidationResult<TOutput>(HttpRequestResult<TOutput> result) => new(result.Output, result.Errors);
    public static implicit operator TOutput(HttpRequestResult<TOutput> result) => result.Output;
    public static implicit operator HttpRequestState(HttpRequestResult<TOutput> result) => result.State;

    public static HttpRequestResult<TOutput> operator +(HttpRequestResult<TOutput> left, IValidationResult? right)
        => right is null ? left : new(left.State, left.Output, left.Errors.Union([.. right.Errors]));
    public static HttpRequestResult<TOutput> operator +(HttpRequestResult<TOutput> left, IEnumerable<ResultError>? right)
        => right is null ? left : new(left.State, left.Output, [.. left.Errors.Union(right)]);
    public static HttpRequestResult<TOutput> operator +(HttpRequestResult<TOutput> left, ResultError? right)
        => right is null ? left : new(left.State, left.Output, left.Errors.Union([right]));
    public static HttpRequestResult<TOutput> operator +(HttpRequestResult<TOutput> left, string? right)
        => right is null ? left : new(left.State, left.Output, left.Errors.Union([right]));

    #pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    public bool Equals(HttpRequestResult<TOutput>? other)
        => other is not null && State == other.State && (Output?.Equals(other.Output) ?? other.Output is null) && Errors.SequenceEqual(other.Errors);
    #pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.
}
