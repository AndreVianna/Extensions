namespace DotNetToolbox;

public static partial class Ensure {
    public static TArgument? IsEqual<TArgument>(TArgument? argument, TArgument? requiredValue, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEquatable<TArgument?> {
        ArgumentOutOfRangeException.ThrowIfNotEqual(argument, requiredValue, paramName);
        return argument;
    }
    public static TArgument? IsNotEqual<TArgument>(TArgument? argument, TArgument? forbiddenValue, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IEquatable<TArgument?> {
        ArgumentOutOfRangeException.ThrowIfEqual(argument, forbiddenValue, paramName);
        return argument;
    }
    public static TArgument IsGreaterThan<TArgument>(TArgument argument, TArgument maximum, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IComparable<TArgument> {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(argument, maximum, paramName);
        return argument;
    }
    public static TArgument IsLessThan<TArgument>(TArgument argument, TArgument minimum, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IComparable<TArgument> {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(argument, minimum, paramName);
        return argument;
    }
    public static TArgument IsNotGreaterThan<TArgument>(TArgument argument, TArgument maximum, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IComparable<TArgument> {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(argument, maximum, paramName);
        return argument;
    }
    public static TArgument IsNotLessThan<TArgument>(TArgument argument, TArgument minimum, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : IComparable<TArgument> {
        ArgumentOutOfRangeException.ThrowIfLessThan(argument, minimum, paramName);
        return argument;
    }
    public static TArgument IsZero<TArgument>(TArgument argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : INumberBase<TArgument> {
        ArgumentOutOfRangeException.ThrowIfNotEqual(argument, TArgument.Zero, paramName);
        return argument;
    }
    public static TArgument IsNotZero<TArgument>(TArgument argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : INumberBase<TArgument> {
        ArgumentOutOfRangeException.ThrowIfZero(argument, paramName);
        return argument;
    }
    // Does not include Zero
    public static TArgument IsPositive<TArgument>(TArgument argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : INumberBase<TArgument>, IComparable<TArgument> {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(argument, paramName);
        return argument;
    }
    // Does not include Zero
    public static TArgument IsNegative<TArgument>(TArgument argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : INumberBase<TArgument>, IComparable<TArgument> {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(argument, TArgument.Zero, paramName);
        return argument;
    }
    public static TArgument IsNotNegative<TArgument>(TArgument argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : INumberBase<TArgument>, IComparable<TArgument> {
        ArgumentOutOfRangeException.ThrowIfNegative(argument, paramName);
        return argument;
    }
    public static TArgument IsNotPositive<TArgument>(TArgument argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        where TArgument : INumberBase<TArgument>, IComparable<TArgument> {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(argument, TArgument.Zero, paramName);
        return argument;
    }
}
