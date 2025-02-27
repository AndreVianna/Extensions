namespace DotNetToolbox.AI.Jobs;

internal static partial class JobInputHelper {
    private static readonly JsonSerializerOptions _jsonOptions = new() {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters = { new JobObjectSerializer() },
    };

    public static string FormatInput(object? input, string? template, Dictionary<Type, Func<object, string>>? converters = null) {
        converters ??= [];
        if (string.IsNullOrWhiteSpace(template)) return ConvertInput(input, converters);
        var keys = ExtractKeys(template);
        if (keys.Length == 0) return template;

        if (IsNotNull(input) is not IEnumerable<KeyValuePair<string, object>> && !input.GetType().IsClass)
            throw new ArgumentException("Input must be either Dictionary<string, object> or an Class.", nameof(input));
        var result = template;
        foreach (var key in keys) {
            var replacement = FindValue(input, key, converters);
            result = result.Replace($"<<{key}>>", replacement);
        }

        return result;
    }

    private static string[] ExtractKeys(string template)
        => [.. MatchInputKey.Matches(template).Select(m => m.Groups[1].Value).Distinct()];

    private static string FindValue(object input, string key, IReadOnlyDictionary<Type, Func<object, string>> converters)
        => input switch {
            IEnumerable<KeyValuePair<string, object>> dict when Map.FromMap(dict).TryGetValue(key, out var value) => ConvertInput(value, converters),
            IEnumerable<KeyValuePair<string, object>> => throw new KeyNotFoundException($"Id '{key}' not found in the input."),
            _ when input.GetType().GetProperty(key) is { } prop => ConvertInput(prop.GetValue(input), converters),
            _ => throw new MissingMemberException($"Property '{key}' not found in the input object."),
        };

    private static string ConvertInput(object? input, IReadOnlyDictionary<Type, Func<object, string>> converters) {
        if (input is null) return string.Empty;
        var inputType = input.GetType();
        return converters.TryGetValue(inputType, out var converter)
                   ? converter(input)
                   : input is IEnumerable<object> list
                       ? string.Join("\n", list.ToArray(v => $" - {ConvertInput(v, converters)}"))
                       : inputType.IsClass
                           ? JsonSerializer.Serialize(input, _jsonOptions)
                           : input.ToString() ?? string.Empty;
    }

    [GeneratedRegex(@"<<(\w+)>>", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    private static partial Regex MatchInputKey { get; }
}
