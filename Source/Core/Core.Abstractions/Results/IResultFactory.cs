namespace DotNetToolbox.Results;

public interface IResultFactory {
    static abstract Result Success();
    static abstract Result Failure(IError error, params IEnumerable<IError> additionalErrors);
    static abstract Result Failure(IEnumerable<IError> errors);
    static abstract Result<TValue> Success<TValue>(TValue value);
    static abstract Result<TValue> Failure<TValue>(TValue value, IError error, params IEnumerable<IError> additionalErrors);
    static abstract Result<TValue> Failure<TValue>(TValue value, IEnumerable<IError> errors);
}
