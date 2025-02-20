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
        var result = argument?.Validate(context) ?? ValidationResult.Success();
        return result.IsSuccess && argument is not null
                   ? argument
                   : defaultValue;
    }

    [return: NotNull]
    public static TArgument IsValid<TArgument>([NotNull] TArgument? argument, Func<TArgument, IValidationResult> validate, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        var result = validate(IsNotNull(argument, paramName));
        return result.IsSuccess
                   ? argument
                   : throw new ResultException(string.Format(null, InvertMessage(ValueIsValid)), paramName!);
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TArgument? DefaultIfNotValid<TArgument>(TArgument? argument, Func<TArgument?, IValidationResult> validate, TArgument? defaultValue = default)
        => validate(argument).IsSuccess && argument is not null
               ? argument
               : defaultValue;

    public static Task<TArgument?> IsValidAsync<TArgument>(TArgument? argument, IMap? context = null, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IAsyncValidatable
        => IsValidAsync(IsNotNull(argument, paramName), arg => arg.ValidateAsync(context), paramName);

    public static async Task<TArgument?> DefaultIfNotValidAsync<TArgument>(TArgument? argument, IMap? context = null, TArgument? defaultValue = default)
        where TArgument : IAsyncValidatable {
        var result = await (argument?.ValidateAsync(context) ?? Task.FromResult(ValidationResult.Success()));
        return result.IsSuccess && argument is not null
                   ? argument
                   : defaultValue;
    }

    public static async Task<TArgument?> IsValidAsync<TArgument>(TArgument? argument, Func<TArgument, Task<IValidationResult>> validate, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        var result = await validate(IsNotNull(argument, paramName));
        return result.IsSuccess
                   ? argument
                   : throw new ResultException(string.Format(null, InvertMessage(ValueIsValid)), paramName!);
    }

    public static async Task<TArgument?> DefaultIfNotValidAsync<TArgument>(TArgument? argument, Func<TArgument?, Task<IValidationResult>> validate, TArgument? defaultValue = default)
        => (await validate(argument)).IsSuccess && argument is not null
               ? argument
               : defaultValue;

    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreValid<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<IValidatable>
        => argument?.All(i => i.Validate().IsSuccess) ?? true
               ? argument
               : throw new ResultException(string.Format(null, InvertMessage(CollectionMustContainInvalid)), paramName!);

    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreValid<TArgument, TValue>(TArgument? argument, Func<TValue?, IValidationResult> validate, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<TValue?>
        => argument?.All(i => validate(i).IsSuccess) ?? true
               ? argument
               : throw new ResultException(string.Format(null, InvertMessage(CollectionMustContainInvalid)), paramName!);
}
