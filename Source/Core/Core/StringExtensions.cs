// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System;

public static class StringExtensions {
    public static string? TrimStart(this string? source, string? value)
        => string.IsNullOrEmpty(source)
        || string.IsNullOrEmpty(value)
        || value.Length > source.Length
        || !source.StartsWith(value)
                   ? source
                   : source[value.Length..];

    public static string? TrimEnd(this string? source, string? value)
        => string.IsNullOrEmpty(source)
        || string.IsNullOrEmpty(value)
        || value.Length > source.Length
        || !source.EndsWith(value)
               ? source
               : source[..^value.Length];

    public static string? Trim(this string? source, string? value)
        => source?.TrimStart(value).TrimEnd(value);
}