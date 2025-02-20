namespace DotNetToolbox.Environment;

public class ApplicationEnvironment(IServiceProvider services, string? name = null, IAssemblyDescriptor? assembly = null, IOperatingSystem? os = null)
    : IApplicationEnvironment {
    public virtual string Name { get; } = name ?? string.Empty;
    public virtual IAssemblyDescriptor Assembly { get; } = assembly ?? services.GetRequiredService<IAssemblyDescriptor>();
    public virtual IOperatingSystem OperatingSystem { get; } = os ?? new OperatingSystem(services);
}
