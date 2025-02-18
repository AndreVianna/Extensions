// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static class Cast {
    public static TTarget To<TTarget>(this object? value) {
        try {
            return (TTarget)Convert.ChangeType(value, typeof(TTarget))!;
        }
        catch (Exception ex) {
            throw new InvalidCastException($"Failed to cast value to type {typeof(TTarget)}", ex);
        }
    }
}
