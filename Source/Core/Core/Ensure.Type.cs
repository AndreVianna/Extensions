namespace DotNetToolbox;

public static partial class Ensure {
    [return: NotNull]
    public static TArgument IsOfType<TArgument>([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        => IsNotNull(argument, paramName) is TArgument result
            ? result
            : throw new ArgumentException(string.Format(null, MustBeOfType, typeof(TArgument).Name, argument.GetType().Name), paramName);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TArgument DefaultIfNotOfType<TArgument>(object? argument, TArgument? defaultValue = default)
        => argument switch {
            TArgument result => result,
            _ => IsNotNull(defaultValue),
        };
}
