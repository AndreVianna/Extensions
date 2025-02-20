namespace DotNetToolbox.Results;

public interface IHasErrors {
    ISet<ResultError> Errors { get; }
    bool HasErrors { get; }
    void EnsureHasNoErrors();
}
