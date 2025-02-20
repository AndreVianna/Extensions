namespace DotNetToolbox.Environment;

public interface IAssemblyAccessor {
    IAssemblyDescriptor GetExecutingAssembly();
    IAssemblyDescriptor? GetEntryAssembly();
    IAssemblyDescriptor? GetDeclaringAssembly(Type type);
    IAssemblyDescriptor? GetDeclaringAssembly<TType>();
    IAssemblyDescriptor GetCallingAssembly();
}
