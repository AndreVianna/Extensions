namespace DotNetToolbox.Results;

public interface IHasErrors {
    IReadOnlySet<Error> Errors { get; }
    bool HasErrors { get; }
}
