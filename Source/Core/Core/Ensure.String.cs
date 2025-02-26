namespace DotNetToolbox;

public static partial class Ensure {
    public static string IsNotNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        ArgumentException.ThrowIfNullOrEmpty(argument, paramName);
        return argument;
    }

    public static string IsNotNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument, paramName);
        return argument;
    }

    [SuppressMessage("Roslynator", "RCS1212:Remove redundant assignment", Justification = "Assignment is required.")]
    public static string DefaultIfNullOrEmpty([NotNull] string? argument, string defaultValue, [CallerArgumentExpression(nameof(defaultValue))] string? paramName = null) {
        argument = string.IsNullOrEmpty(argument) ? IsNotNullOrEmpty(defaultValue, paramName) : argument;
        return argument;
    }

    [SuppressMessage("Roslynator", "RCS1212:Remove redundant assignment", Justification = "Assignment is required.")]
    public static string DefaultIfNullOrWhiteSpace([NotNull] string? argument, string defaultValue, [CallerArgumentExpression(nameof(defaultValue))] string? paramName = null) {
        argument = string.IsNullOrWhiteSpace(argument) ? IsNotNullOrWhiteSpace(defaultValue, paramName) : argument;
        return argument;
    }

    public static FormattableString IsNotNullOrEmpty([NotNull] FormattableString? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        ArgumentException.ThrowIfNullOrEmpty(argument?.Format, paramName);
        return argument;
    }

    public static FormattableString IsNotNullOrWhiteSpace([NotNull] FormattableString? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument?.Format, paramName);
        return argument;
    }

    [SuppressMessage("Roslynator", "RCS1212:Remove redundant assignment", Justification = "Assignment is required.")]
    public static FormattableString DefaultIfNullOrEmpty([NotNull] FormattableString? argument, FormattableString defaultValue, [CallerArgumentExpression(nameof(defaultValue))] string? paramName = null) {
        argument = string.IsNullOrEmpty(argument?.Format) ? IsNotNullOrEmpty(defaultValue, paramName) : argument;
        return argument;
    }

    [SuppressMessage("Roslynator", "RCS1212:Remove redundant assignment", Justification = "Assignment is required.")]
    public static FormattableString DefaultIfNullOrWhiteSpace([NotNull] FormattableString? argument, FormattableString defaultValue, [CallerArgumentExpression(nameof(defaultValue))] string? paramName = null) {
        argument = string.IsNullOrWhiteSpace(argument?.Format) ? IsNotNullOrWhiteSpace(defaultValue, paramName) : argument;
        return argument;
    }
}
