namespace DotNetToolbox.Results;

public sealed class ResultException
    : Exception {
    private const string _defaultMessage = "An error has occured.";

    public IReadOnlyList<IError> Errors { get; }

    public ResultException(string? message = null)
        : base(message ?? _defaultMessage) {
        Errors = [new Error(message ?? _defaultMessage)];
    }

    public ResultException(Exception innerException)
        : this(_defaultMessage, innerException) {
    }

    public ResultException(IError error, Exception? innerException = null)
        : this(error.Message, [error], innerException) {
    }

    public ResultException(IEnumerable<IError> errors, Exception? innerException = null)
        : this(_defaultMessage, errors, innerException) {
    }

    public ResultException(string message, Exception? innerException = null)
        : this(message, [], innerException) {
    }

    public ResultException(string message, IError error, Exception? innerException = null)
        : this(message, [error], innerException) {
    }

    public ResultException(string message, IEnumerable<IError> errors, Exception? innerException = null)
        : base(message, innerException) {
        Errors = [.. errors.Distinct()];
    }
}
