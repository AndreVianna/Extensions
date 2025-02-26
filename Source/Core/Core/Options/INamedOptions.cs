namespace DotNetToolbox.Options;

public interface INamedOptions<out TOptions>
    : IHasDefault<TOptions>
    where TOptions : INamedOptions<TOptions> {
    static abstract string SectionName { get; }
}
