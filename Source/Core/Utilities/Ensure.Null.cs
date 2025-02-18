// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static partial class Ensure {
    [return: NotNull]
    public static TArgument IsNotNull<TArgument>([NotNull] TArgument? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null) {
        ArgumentNullException.ThrowIfNull(argument, paramName);
        return argument;
    }

    [return: NotNull]
    public static TArgument DefaultIfNull<TArgument>([NotNull] TArgument? argument, TArgument defaultValue) {
        argument ??= IsNotNull(defaultValue);
        return argument;
    }
}
