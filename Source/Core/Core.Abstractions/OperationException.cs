namespace DotNetToolbox;

public sealed class OperationException
    : Exception {
    public const string DefaultMessage = "Operation failed.";

    public OperationError[] Errors { get; }

    public OperationException(string? message = null)
        : base(message ?? DefaultMessage) {
        Errors = [new(message ?? DefaultMessage)];
    }

    public OperationException(Exception innerException)
        : this(DefaultMessage, innerException) {
    }

    public OperationException(OperationError error, Exception? innerException = null)
        : this(DefaultMessage, error, innerException) {
    }

    public OperationException(IEnumerable<OperationError> errors, Exception? innerException = null)
        : this(DefaultMessage, errors, innerException) {
    }

    public OperationException(string message, Exception? innerException = null)
        : this(message, [], innerException) {
    }

    public OperationException(string message, OperationError error, Exception? innerException = null)
        : this(message, [error], innerException) {
    }

    public OperationException(string message, IEnumerable<OperationError> errors, Exception? innerException = null)
        : base(!string.IsNullOrWhiteSpace(message) ? message : throw new ArgumentException("The error message cannot be null or whitespace.", nameof(message)), innerException) {
        Errors = [.. errors.Distinct()];
    }
}
