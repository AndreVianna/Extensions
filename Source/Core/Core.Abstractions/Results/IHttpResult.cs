namespace DotNetToolbox.Results;

public interface IHttpResult
    : IHasErrors {
    bool IsOk { get; } // 200
    bool IsCreated { get; } // 201
    bool IsBadRequest { get; } // 400
    bool IsUnauthorized { get; } // 401
    bool IsNotFound { get; } // 404
    bool IsConflict { get; } // 409
    bool IsServerError { get; } // 500
}

public interface IHttpResult<TValue>
    : IHttpResult
    , IReturnsValue<TValue>;
