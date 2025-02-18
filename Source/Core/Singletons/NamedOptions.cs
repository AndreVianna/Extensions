namespace DotNetToolbox.Singleton;

public class NamedOptions<TOptions>
    : HasDefault<TOptions>
    , INamedOptions<TOptions>
    where TOptions : NamedOptions<TOptions>, new() {
    private const string _suffix = "Options";
    private static readonly string _typeName = typeof(TOptions).Name;

    // ReSharper disable once StaticMemberInGenericType
    [SuppressMessage("Roslynator", "RCS1158:Static member in generic type should use a type parameter", Justification = "Unnecessary here.")]
    public static string SectionName { get; }
        = _typeName.EndsWith(_suffix)
              ? _typeName.Remove(_typeName.Length - _suffix.Length)
              : _typeName;
}
