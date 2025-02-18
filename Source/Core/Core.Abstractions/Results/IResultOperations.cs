namespace DotNetToolbox.Results;

public interface IResultOperations<TSelf>
    : IAdditiveIdentity<TSelf, TSelf>,
      IAdditionOperators<TSelf, IResult?, TSelf>,
      IAdditionOperators<TSelf, IEnumerable<OperationError>?, TSelf>,
      IAdditionOperators<TSelf, OperationError?, TSelf>,
      IAdditionOperators<TSelf, string?, TSelf>
    where TSelf : IResultOperations<TSelf> {
    static abstract implicit operator TSelf(OperationErrors errors);
    static abstract implicit operator TSelf(HashSet<OperationError> errors);
    static abstract implicit operator TSelf(List<OperationError> errors);
    static abstract implicit operator TSelf(OperationError[] errors);
    static abstract implicit operator TSelf(OperationError error);
    static abstract implicit operator TSelf(string error);
    static abstract implicit operator OperationError[](TSelf self);
    static abstract implicit operator List<OperationError>(TSelf self);
    static abstract implicit operator HashSet<OperationError>(TSelf self);
    static abstract implicit operator OperationErrors(TSelf self);
};

public interface IResultOperations<TSelf, TValue>
    : IResultOperations<TSelf>
    where TSelf : IResultOperations<TSelf, TValue> {
    static abstract implicit operator TSelf(TValue value);
    static abstract implicit operator TValue(TSelf self);
};
