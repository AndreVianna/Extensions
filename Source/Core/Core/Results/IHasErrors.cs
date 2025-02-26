namespace DotNetToolbox.Results;

public interface IHasErrors {
    IReadOnlySet<IError> Errors { get; }
    bool HasErrors { get; }
}
