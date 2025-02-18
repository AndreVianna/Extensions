namespace DotNetToolbox.Results;

public static class HttpRequestResultFactory {
    private static IHttpResult Create(HttpRequestState type, params IEnumerable<OperationError> errors)
        => new HttpRequestResult(type, errors);
    private static IHttpResult Create<TValue>(HttpRequestState type, TValue value, params IEnumerable<OperationError> errors)
        => new HttpResult<TValue>(type, value, errors);

    public static IHttpResult Ok()
        => Create(HttpRequestState.Ok);
    public static IHttpResult Ok<TValue>(TValue value)
        => Create(HttpRequestState.Ok, value);
    public static IHttpResult Created()
        => Create(HttpRequestState.Created);
    public static IHttpResult Created<TValue>(TValue value)
        => Create(HttpRequestState.Created, value);
    public static IHttpResult BadRequest(params IEnumerable<OperationError> errors)
        => Create(HttpRequestState.BadRequest, errors);
    public static IHttpResult BadRequest(string message, string? source = null)
        => BadRequest(new OperationError(message, source ?? string.Empty));
    public static IHttpResult Unauthorized()
        => Create(HttpRequestState.Unauthorized);
    public static IHttpResult NotFound(params IEnumerable<OperationError> errors)
        => Create(HttpRequestState.NotFound, errors);
    public static IHttpResult NotFound(string message, string? source = null)
        => BadRequest(new OperationError(message, source ?? string.Empty));
    public static IHttpResult Conflict(params IEnumerable<OperationError> errors)
        => Create(HttpRequestState.Conflict, errors);
    public static IHttpResult Conflict(string message, string? source = null)
        => Conflict(new OperationError(message, source ?? string.Empty));
    public static IHttpResult Conflict<TValue>(TValue value, params IEnumerable<OperationError> errors)
        => Create(HttpRequestState.Conflict, value, errors);
    public static IHttpResult Conflict<TValue>(TValue value, string message, string? source = null)
        => Conflict(value, new OperationError(message, source ?? string.Empty));
    public static IHttpResult ServerError(params IEnumerable<OperationError> errors)
        => Create(HttpRequestState.ServerError, errors);
    public static IHttpResult ServerError(string message, string? source = null)
        => ServerError(new OperationError(message, source ?? string.Empty));
}
