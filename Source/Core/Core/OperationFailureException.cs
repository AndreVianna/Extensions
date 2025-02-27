namespace DotNetToolbox;

// ReSharper disable once MemberCanBeInternal - Exposed for use in other projects
public sealed class OperationFailureException
    : InvalidOperationException {
    private const string _defaultMessage = "An error has occured.";

    public IReadOnlyList<Error> Errors { get; }

    public OperationFailureException(string? message = null)
        : base(message ?? _defaultMessage) {
        Errors = [new Error(message ?? _defaultMessage)];
    }

    public OperationFailureException(Exception innerException)
        : this(_defaultMessage, innerException) {
    }

    public OperationFailureException(Error error, Exception? innerException = null)
        : this(error.Message, [error], innerException) {
    }

    public OperationFailureException(IEnumerable<Error> errors, Exception? innerException = null)
        : this(_defaultMessage, errors, innerException) {
    }

    public OperationFailureException(string message, Exception? innerException = null)
        : this(message, [], innerException) {
    }

    public OperationFailureException(string message, Error error, Exception? innerException = null)
        : this(message, [error], innerException) {
    }

    public OperationFailureException(string message, IEnumerable<Error> errors, Exception? innerException = null)
        : base(message, innerException) {
        Errors = [.. errors.Distinct()];
    }
}
