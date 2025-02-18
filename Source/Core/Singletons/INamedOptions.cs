namespace DotNetToolbox.Singleton;

public interface INamedOptions<out TOptions>
    : IHasDefault<TOptions>
    where TOptions : INamedOptions<TOptions> {
    public static abstract string SectionName { get; }
}
