namespace DotNetToolbox.Options;

public interface INamedOptions<out TOptions>
    : Results.IHasDefault<TOptions>
    where TOptions : INamedOptions<TOptions> {
    static abstract string SectionName { get; }
}
