namespace DotNetToolbox;

public static partial class Ensure {
    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreNotNull<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable
        => argument?.Cast<object?>().All(static i => i is not null) ?? true
               ? argument
               : throw new OperationFailureException(new Error(string.Format(null, BuildMessage(paramName!, CannotContainNulls)), paramName!));

    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreValid<TArgument, TValue>(TArgument? argument, Func<TValue?, bool> isValid, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<TValue?>
        => argument?.All(isValid) ?? true
                   ? argument
                   : throw new OperationFailureException(new Error(string.Format(null, BuildMessage(paramName!, CannotContainInvalid)), paramName!));
}
