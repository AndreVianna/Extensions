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
        var result = argument?.Validate(context) ?? Result.Success();
        return result.IsSuccessful && argument is not null
                   ? argument
                   : defaultValue;
    }

    [return: NotNull]
    public static TArgument IsValid<TArgument>([NotNull] TArgument? argument, Func<TArgument, IResult> validate, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        var result = validate(IsNotNull(argument, paramName));
        return result.IsSuccessful
                   ? argument
                   : throw new ResultException(new Error(string.Format(null, InvertMessage(ValueIsValid)), paramName!));
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TArgument? DefaultIfNotValid<TArgument>(TArgument? argument, Func<TArgument?, Result> validate, TArgument? defaultValue = default)
        => validate(argument).IsSuccessful && argument is not null
               ? argument
               : defaultValue;

    public static Task<TArgument?> IsValidAsync<TArgument>(TArgument? argument, IMap? context = null, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IAsyncValidatable
        => IsValidAsync(IsNotNull(argument, paramName), arg => arg.ValidateAsync(context), paramName);

    public static async Task<TArgument?> DefaultIfNotValidAsync<TArgument>(TArgument? argument, IMap? context = null, TArgument? defaultValue = default)
        where TArgument : IAsyncValidatable {
        var result = await (argument?.ValidateAsync(context) ?? Task.FromResult(Result.Success()));
        return result.IsSuccessful && argument is not null
                   ? argument
                   : defaultValue;
    }

    public static async Task<TArgument?> IsValidAsync<TArgument>(TArgument? argument, Func<TArgument, Task<IResult>> validate, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        var result = await validate(IsNotNull(argument, paramName));
        return result.IsSuccessful
                   ? argument
                   : throw new ResultException(new Error(string.Format(null, InvertMessage(ValueIsValid)), paramName!));
    }

    public static async Task<TArgument?> DefaultIfNotValidAsync<TArgument>(TArgument? argument, Func<TArgument?, Task<IResult>> validate, TArgument? defaultValue = default)
        => (await validate(argument)).IsSuccessful && argument is not null
               ? argument
               : defaultValue;

    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreValid<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<IValidatable>
        => argument?.All(i => i.Validate().IsSuccessful) ?? true
               ? argument
               : throw new ResultException(new Error(string.Format(null, InvertMessage(CollectionMustContainInvalid)), paramName!));

    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreValid<TArgument, TValue>(TArgument? argument, Func<TValue?, IResult> validate, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<TValue?>
        => argument?.All(i => validate(i).IsSuccessful) ?? true
               ? argument
               : throw new ResultException(new Error(string.Format(null, InvertMessage(CollectionMustContainInvalid)), paramName!));
}
