namespace DotNetToolbox.Results;

public interface IHasErrors {
    IEnumerable<OperationError> Errors { get; }
    bool HasErrors { get; }
    void EnsureHasNoErrors();
}