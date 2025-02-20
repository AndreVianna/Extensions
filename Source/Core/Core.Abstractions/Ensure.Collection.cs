// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static partial class Ensure {
    [return: NotNullIfNotNull(nameof(argument))]
    public static TArgument? IsNotEmpty<TArgument>(TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable
        => argument switch {
            ICollection { Count: 0 } => throw new ArgumentException(string.Format(null, InvertMessage(CollectionMustBeEmpty)), paramName),
            not null when !argument.Cast<object>().Any() => throw new ArgumentException(string.Format(null, InvertMessage(CollectionMustBeEmpty)), paramName),
            _ => argument,
        };

    [return: NotNull]
    public static TArgument IsNotNullOrEmpty<TArgument>([NotNull] TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEnumerable
        => IsNotEmpty(IsNotNull(argument, paramName), paramName);
}
