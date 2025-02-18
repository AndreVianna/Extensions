using static DotNetToolbox.Results.ResultFactory;

// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static partial class Ensure {
    [return: NotNull]
    public static TArgument IsValid<TArgument>([NotNull] TArgument? argument, IMap? context = null, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IValidatable
        => IsValid(IsNotNull(argument, paramName), arg => arg.Validate(context), paramName);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TArgument? DefaultIfNotValid<TArgument>(TArgument? argument, IMap? context = null, TArgument? defaultValue = default)
        where TArgument : IValidatable {
        var result = argument?.Validate(context) ?? Success();
        return result.IsSuccess && argument is not null
                   ? argument
                   : defaultValue;
    }

    [return: NotNull]
    public static TArgument IsValid<TArgument>([NotNull] TArgument? argument, Func<TArgument, IResult> validate, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        var result = validate(IsNotNull(argument, paramName));
        return result.IsSuccess
            ? argument
            : throw new OperationException(string.Format(null, InvertMessage(ValueIsValid)), paramName!);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TArgument? DefaultIfNotValid<TArgument>(TArgument? argument, Func<TArgument?, IResult> validate, TArgument? defaultValue = default)
        => validate(argument).IsSuccess && argument is not null
               ? argument
               : defaultValue;

    [return: NotNull]
    public static TArgument IsValid<TArgument>([NotNull] TArgument? argument, Func<TArgument?, bool> isValid, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        => isValid(IsNotNull(argument, paramName))
               ? argument
               : throw new OperationException(string.Format(null, InvertMessage(ValueIsValid)), paramName!);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TArgument? DefaultIfNotValid<TArgument>(TArgument? argument, Func<TArgument?, bool> isValid, TArgument? defaultValue = default)
        => isValid(argument)
               ? argument ?? defaultValue
               : defaultValue;
}
