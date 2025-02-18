namespace DotNetToolbox.Results;

public static class ResultFactory {
    public static IResult Create(params IEnumerable<OperationError> errors)
        => new Result(errors);
    public static IResult Create<TValue>(TValue value, params IEnumerable<OperationError> errors)
        => new Result<TValue>(value, errors);

    public static IResult New()
        => Create();
    public static IResult Success()
        => Create();
    public static IResult Success<TValue>(TValue value)
        => Create(value);
    public static IResult Failure(IEnumerable<OperationError> errors)
        => Create(errors);
    public static IResult Failure(OperationError error, params IEnumerable<OperationError> otherErrors)
        => Failure([error, .. otherErrors]);
    public static IResult Failure(string message, string? source = null)
        => Failure(new OperationError(message, source ?? string.Empty));
}
