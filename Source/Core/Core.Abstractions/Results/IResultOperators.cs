namespace DotNetToolbox.Results;

public interface IResultOperators<TSelf>
    : IAdditionOperators<TSelf, IValidationResult?, TSelf>,
      IAdditionOperators<TSelf, IEnumerable<ResultError>?, TSelf>,
      IAdditionOperators<TSelf, ResultError?, TSelf>,
      IAdditionOperators<TSelf, string?, TSelf>
    where TSelf : IResultOperators<TSelf> {
    static abstract implicit operator TSelf(ResultErrors errors);
    static abstract implicit operator TSelf(HashSet<ResultError> errors);
    static abstract implicit operator TSelf(List<ResultError> errors);
    static abstract implicit operator TSelf(ResultError[] errors);
    static abstract implicit operator TSelf(ResultError error);
    static abstract implicit operator TSelf(string error);
    static abstract implicit operator ResultError[](TSelf self);
    static abstract implicit operator List<ResultError>(TSelf self);
    static abstract implicit operator HashSet<ResultError>(TSelf self);
    static abstract implicit operator ResultErrors(TSelf self);
};

public interface IResultOperators<TSelf, TOutput>
    : IResultOperators<TSelf>
    where TSelf : IResultOperators<TSelf, TOutput> {
    static abstract implicit operator TSelf(TOutput value);
    static abstract implicit operator TOutput(TSelf self);
};
