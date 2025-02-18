namespace DotNetToolbox.Environment;

public class OperatingSystem(IServiceProvider services, string? name = null, Version? version = null, string? servicePack = null, string? eol = null)
    : IOperatingSystem {
    public virtual string Name { get; } = name ?? $"{System.Environment.OSVersion.Platform}";
    public virtual Version Version { get; } = version ?? System.Environment.OSVersion.Version;
    public virtual string ServicePack { get; } = servicePack ?? $"{System.Environment.OSVersion.ServicePack}";
    public virtual string EndOfLine { get; } = eol ?? System.Environment.NewLine;
    public virtual IOutput Output { get; } = services.GetRequiredService<IOutput>();
    public virtual IInput Input { get; } = services.GetRequiredService<IInput>();
    public virtual IDateTimeProvider DateTime { get; } = services.GetRequiredService<IDateTimeProvider>();
    public virtual IGuidProvider Guid { get; } = services.GetRequiredService<IGuidProvider>();
    public virtual IFileSystemAccessor FileSystem { get; } = services.GetRequiredService<IFileSystemAccessor>();
}
