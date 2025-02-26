namespace DotNetToolbox.Options;

public class NamedOptions<TOptions>
    : INamedOptions<TOptions>
    where TOptions : NamedOptions<TOptions>, new() {
    public static TOptions Default { get; } = new();

    private const string _suffix = "Options";

    // ReSharper disable once StaticMemberInGenericType
    public static string SectionName {
        get => field.EndsWith(_suffix)
            && field.Length > _suffix.Length
                    ? field[..^_suffix.Length]
                    : field;
    } = typeof(TOptions).Name;
}
