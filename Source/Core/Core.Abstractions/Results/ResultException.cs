namespace DotNetToolbox.Results;

public sealed class ResultException
    : Exception {
    private const string _defaultMessage = "Operation failed.";

    public ResultError[] Errors { get; }

    public ResultException(string? message = null)
        : base(message ?? _defaultMessage) {
        Errors = [new(message ?? _defaultMessage)];
    }

    public ResultException(Exception innerException)
        : this(_defaultMessage, innerException) {
    }

    public ResultException(ResultError error, Exception? innerException = null)
        : this(_defaultMessage, error, innerException) {
    }

    public ResultException(IEnumerable<ResultError> errors, Exception? innerException = null)
        : this(_defaultMessage, errors, innerException) {
    }

    public ResultException(string message, Exception? innerException = null)
        : this(message, [], innerException) {
    }

    public ResultException(string message, ResultError error, Exception? innerException = null)
        : this(message, [error], innerException) {
    }

    public ResultException(string message, IEnumerable<ResultError> errors, Exception? innerException = null)
        : base(!string.IsNullOrWhiteSpace(message) ? message : throw new ArgumentException("The error message cannot be null or whitespace.", nameof(message)), innerException) {
        Errors = [.. errors.Distinct()];
    }
}
