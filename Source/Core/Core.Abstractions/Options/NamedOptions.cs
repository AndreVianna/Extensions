namespace DotNetToolbox.Options;

public class NamedOptions<TOptions>
    : HasDefault<TOptions>
    , INamedOptions<TOptions>
    where TOptions : NamedOptions<TOptions>, new() {
    private const string _suffix = "Options";
    private static readonly string _typeName = typeof(TOptions).Name;

    // ReSharper disable once StaticMemberInGenericType
    [SuppressMessage("Roslynator", "RCS1158:Static member in generic type should use a type parameter", Justification = "<Pending>")]
    public static string SectionName { get; }
        = _typeName.EndsWith(_suffix)
              ? _typeName[..^_suffix.Length]
              : _typeName;
}
