namespace DotNetToolbox.Results;

public interface ITypedResultFactory<TStatus>
    where TStatus : Enum {
    static abstract TypedResult<TStatus> As(TStatus status, params IEnumerable<IError> errors);
    static abstract TypedResult<TStatus, TValue> As<TValue>(TStatus status, TValue value, params IEnumerable<IError> errors);
}
