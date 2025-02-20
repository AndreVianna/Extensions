namespace DotNetToolbox.Results;

public interface IValidationResult
    : IHasErrors {
    bool IsSuccess { get; }
    bool IsFailure { get; }
    IValidationResult<TOutput> SetOutput<TOutput>(TOutput output);
    IValidationResult Add(IValidationResult other);
};

public interface IValidationResult<TOutput>
    : IValidationResult
    , IHasOutput<TOutput> {
    new IValidationResult<TOutput>  MergeWith(IValidationResult other);
}
