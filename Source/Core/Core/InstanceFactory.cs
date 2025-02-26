using static System.Activator;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;
using static Microsoft.Extensions.DependencyInjection.ActivatorUtilities;

using Has = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;

// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static class InstanceFactory {
    private const BindingFlags _allConstructors = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance;

    public static T? CreateOrDefault<[Has(PublicConstructors | NonPublicConstructors)] T>(params object[] args)
        where T : class
        => TryCreate<T>(out var result, args) ? result : null;

    public static bool TryCreate<[Has(PublicConstructors | NonPublicConstructors)] T>([MaybeNullWhen(false)] out T instance, params object[] args)
        where T : class {
        try {
            instance = (T)IsNotNull(CreateInstance(typeof(T), _allConstructors, null, args, null, null), typeof(T).Name);
            return true;
        }
        catch {
            instance = null;
            return false;
        }
    }

    [return: NotNull]
    public static T Create<[Has(PublicConstructors | NonPublicConstructors)] T>(params object[] args)
        where T : class {
        try {
            return (T)IsNotNull(CreateInstance(typeof(T), _allConstructors, null, args, null, null), typeof(T).Name);
        }
        catch (Exception ex) {
            throw new InvalidOperationException($"Failed to create instance of type {typeof(T).Name}", ex);
        }
    }

    public static T? CreateOrDefault<[Has(PublicConstructors | NonPublicConstructors)] T>(IServiceProvider services, params object[] args)
        where T : class
        => TryCreate<T>(services, out var result, args) ? result : null;

    public static bool TryCreate<[Has(PublicConstructors)] T>(IServiceProvider services, [MaybeNullWhen(false)] out T instance, params object[] args)
        where T : class {
        try {
            instance = CreateInstance<T>(services, args);
            return true;
        }
        catch {
            instance = null;
            return false;
        }
    }

    [return: NotNull]
    public static T Create<[Has(PublicConstructors)] T>(IServiceProvider services, params object[] args)
        where T : class {
        try {
            return CreateInstance<T>(services, args);
        }
        catch (Exception ex) {
            throw new InvalidOperationException($"Failed to create instance of type {typeof(T).Name}", ex);
        }
    }
}
