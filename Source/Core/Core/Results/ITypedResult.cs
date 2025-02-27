namespace DotNetToolbox.Results;

public interface ITypedResultBase<in TStatus>
    : IResultBase
    where TStatus : Enum {
    bool Is(TStatus status);
    void EnsureIs(TStatus status);
}

public interface ITypedResult<TStatus>
    : ITypedResultBase<TStatus>
    where TStatus : Enum {
    TypedResult<TStatus, TValue> With<TValue>(TValue value);
    TypedResult<TStatus, TValue> WithNo<TValue>();
    TStatus Status { get; }
}

public interface ITypedResult<in TStatus, out TValue>
    : ITypedResultBase<TStatus>
    , IHasValue<TValue>
    where TStatus : Enum;
