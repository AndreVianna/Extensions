namespace DotNetToolbox.Environment;

public interface IApplicationEnvironment {
    string Name { get; }
    IAssemblyDescriptor Assembly { get; }
    IOperatingSystem OperatingSystem { get; }
}
