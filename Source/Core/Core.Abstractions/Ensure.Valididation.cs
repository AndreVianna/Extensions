// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static partial class Ensure {
    [return: NotNull]
    public static TArgument IsValid<TArgument>([NotNull] TArgument? argument, Func<TArgument?, bool> isValid, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        => isValid(IsNotNull(argument, paramName))
               ? argument
               : throw new ResultException(new Error(string.Format(null, InvertMessage(ValueIsValid)), paramName!));

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TArgument? DefaultIfNotValid<TArgument>(TArgument? argument, Func<TArgument?, bool> isValid, TArgument? defaultValue = default)
        => isValid(argument)
               ? argument ?? defaultValue
               : defaultValue;
}
