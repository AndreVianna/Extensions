namespace DotNetToolbox.Results;

public interface ITypedResult<in TStatus>
    : IHasErrors
    where TStatus : Enum {
    bool Is(TStatus status);
    void EnsureIs(TStatus status);
}

public interface ITypedResult<in TStatus, out TValue>
    : ITypedResult<TStatus>
    , IHasValue<TValue>
    where TStatus : Enum;
