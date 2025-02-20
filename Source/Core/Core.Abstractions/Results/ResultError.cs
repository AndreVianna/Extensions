namespace DotNetToolbox.Results;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public sealed record ResultError {
    private readonly string _buffer;
    public const string DefaultErrorMessage = "The value is invalid.";

    public ResultError()
        : this(DefaultErrorMessage, string.Empty) {
    }

    public ResultError(string message, params IEnumerable<string> sources) {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Message = message.Trim();
        Sources = [.. sources.Select(s => s.Trim()).Where(s => s.Length > 0).Distinct().Order()];
        _buffer = $"{Message};{string.Join(';', Sources)}";
    }

    public string Message { get; }
    public string[] Sources { get; }

    [return: NotNullIfNotNull(nameof(message))]
    public static implicit operator ResultError?(string? message)
        => message is null ? null : new(message);

    public static ResultError operator +(ResultError left, string? right)
        => right is null ? left : new(left.Message, [.. left.Sources, right]);

    public bool Equals(ResultError? other)
        => _buffer.Equals(other?._buffer, StringComparison.Ordinal);

    public override int GetHashCode()
        => _buffer.GetHashCode();

    public override string ToString()
        => DebuggerDisplay;

    private string DebuggerDisplay
        => $"""
            {nameof(ResultError)}
            {nameof(Message)}: {Message}
            {nameof(Sources)}: {string.Join(", ", Sources)}
            """;
}
