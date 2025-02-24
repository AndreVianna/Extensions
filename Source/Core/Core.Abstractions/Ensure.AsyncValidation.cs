// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static partial class Ensure {
    public static async Task<TArgument?> IsValidAsync<TArgument>(TArgument? argument, Func<TArgument?, Task<bool>> isValid, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        => await isValid(IsNotNull(argument, paramName))
               ? argument
               : throw new ResultException(new Error(string.Format(null, InvertMessage(ValueIsValid)), paramName!));

    public static async Task<TArgument?> DefaultIfNotValidAsync<TArgument>(TArgument? argument, Func<TArgument?, Task<bool>> isValid, TArgument? defaultValue = default)
        => await isValid(argument) && argument is not null
               ? argument
               : defaultValue;
}
