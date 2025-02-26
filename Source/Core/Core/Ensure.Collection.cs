namespace DotNetToolbox;

public static partial class Ensure {
    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? IsNotEmpty<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable
        => argument switch {
            ICollection { Count: 0 } => throw new ArgumentException(BuildMessage(paramName!, CannotBeEmpty), paramName),
            not null when !argument.Cast<object>().Any() => throw new ArgumentException(BuildMessage(paramName!, CannotBeEmpty), paramName),
            _ => argument,
        };

    [return: NotNull]
    public static TArgument IsNotNullOrEmpty<TArgument>([NotNull] TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable
        => IsNotEmpty(IsNotNull(argument, paramName), paramName);
}
