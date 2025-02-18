// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static partial class Ensure {
    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreNotNull<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable
        => argument?.Cast<object?>().All(i => i is not null) ?? true
               ? argument
               : throw new OperationException(string.Format(null, InvertMessage(CollectionMustContainNull)), paramName!);

    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreValid<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<IValidatable>
        => argument?.All(i => i.Validate().IsSuccess) ?? true
               ? argument
               : throw new OperationException(string.Format(null, InvertMessage(CollectionMustContainInvalid)), paramName!);

    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreValid<TArgument, TValue>(TArgument? argument, Func<TValue?, IResult> validate, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<TValue?>
        => argument?.All(i => validate(i).IsSuccess) ?? true
               ? argument
               : throw new OperationException(string.Format(null, InvertMessage(CollectionMustContainInvalid)), paramName!);

    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? ItemsAreValid<TArgument, TValue>(TArgument? argument, Func<TValue?, bool> isValid, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable<TValue?>
        => argument?.All(isValid) ?? true
                   ? argument
                   : throw new OperationException(string.Format(null, InvertMessage(CollectionMustContainInvalid)), paramName!);
}
