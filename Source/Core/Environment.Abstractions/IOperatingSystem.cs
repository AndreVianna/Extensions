namespace DotNetToolbox.Environment;

public interface IOperatingSystem {
    string Name { get; }
    Version Version { get; }
    string ServicePack { get; }
    string EndOfLine { get; }
    IDateTimeProvider DateTime { get; }
    IFileSystemAccessor FileSystem { get; }
    IGuidProvider Guid { get; }
    IInput Input { get; }
    IOutput Output { get; }
}
