using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static class InstanceFactory {
    private const BindingFlags _allConstructors = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance;

    public static T? CreateOrDefault<[DynamicallyAccessedMembers(PublicConstructors | NonPublicConstructors)] T>(params object[] args)
        where T : class
        => TryCreate<T>(out var result, args) ? result : default;

    public static bool TryCreate<[DynamicallyAccessedMembers(PublicConstructors | NonPublicConstructors)] T>([MaybeNullWhen(false)] out T instance, params object[] args)
        where T : class {
        try {
            instance = (T)IsNotNull(Activator.CreateInstance(typeof(T), _allConstructors, null, args, null, null), typeof(T).Name);
            return true;
        }
        catch {
            instance = default;
            return false;
        }
    }

    [return: NotNull]
    public static T Create<[DynamicallyAccessedMembers(PublicConstructors | NonPublicConstructors)] T>(params object[] args)
        where T : class {
        try {
            return (T)IsNotNull(Activator.CreateInstance(typeof(T), _allConstructors, null, args, null, null), typeof(T).Name);
        }
        catch (Exception ex) {
            throw new InvalidOperationException($"Failed to create instance of type {typeof(T).Name}", ex);
        }
    }

    public static T? CreateOrDefault<[DynamicallyAccessedMembers(PublicConstructors | NonPublicConstructors)] T>(IServiceProvider services, params object[] args)
        where T : class
        => TryCreate<T>(services, out var result, args) ? result : default;

    public static bool TryCreate<[DynamicallyAccessedMembers(PublicConstructors)] T>(IServiceProvider services, [MaybeNullWhen(false)] out T instance, params object[] args)
        where T : class {
        try {
            instance = ActivatorUtilities.CreateInstance<T>(services, args);
            return true;
        }
        catch {
            instance = default;
            return false;
        }
    }

    [return: NotNull]
    public static T Create<[DynamicallyAccessedMembers(PublicConstructors)] T>(IServiceProvider services, params object[] args)
        where T : class {
        try {
            return ActivatorUtilities.CreateInstance<T>(services, args);
        }
        catch (Exception ex) {
            throw new InvalidOperationException($"Failed to create instance of type {typeof(T).Name}", ex);
        }
    }
}
