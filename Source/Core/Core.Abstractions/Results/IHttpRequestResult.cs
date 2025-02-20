namespace DotNetToolbox.Results;

public interface IHttpRequestResult
    : IHasErrors {
    bool IsOk { get; } // 200
    bool IsCreated { get; } // 201
    bool IsBadRequest { get; } // 400
    bool IsUnauthorized { get; } // 401
    bool IsNotFound { get; } // 404
    bool IsConflict { get; } // 409
    bool IsServerError { get; } // 500
    IHttpRequestResult<TOutput> SetOutput<TOutput>(TOutput output);
    IHttpRequestResult MergeWith(IValidationResult other);
}

public interface IHttpRequestResult<TOutput>
    : IHttpRequestResult, IHasOutput<TOutput> {
    new IHttpRequestResult<TOutput> MergeWith(IValidationResult other);
}
