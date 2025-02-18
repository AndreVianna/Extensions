namespace DotNetToolbox.Results;

public static class CrudResultFactory {
    private static ICrudResult Create(CrudState state, params IEnumerable<OperationError> errors)
        => new CrudResult(state, errors);
    private static ICrudResult<TValue> Create<TValue>(CrudState state, TValue value, params IEnumerable<OperationError> errors)
        => new CrudResult<TValue>(state, value, errors);

    public static ICrudResult New()
        => new CrudResult();
    public static ICrudResult Success()
        => Create(CrudState.Success);
    public static ICrudResult<TValue> Success<TValue>(TValue value)
        => Create(CrudState.Success, value);
    public static ICrudResult Invalid(params IEnumerable<OperationError> errors)
        => Create(CrudState.Invalid, errors);
    public static ICrudResult Invalid(string message, string? source = null)
        => Invalid(new OperationError(message, source ?? string.Empty));
    public static ICrudResult NotFound(params IEnumerable<OperationError> errors)
        => Create(CrudState.NotFound, errors);
    public static ICrudResult NotFound(string message, string? source = null)
        => Conflict(new OperationError(message, source ?? string.Empty));
    public static ICrudResult Conflict(params IEnumerable<OperationError> errors)
        => Create(CrudState.Conflict, errors);
    public static ICrudResult Conflict(string message, string? source = null)
        => Conflict(new OperationError(message, source ?? string.Empty));
}
