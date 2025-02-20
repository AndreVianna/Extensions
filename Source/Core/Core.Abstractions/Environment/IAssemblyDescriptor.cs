namespace DotNetToolbox.Environment;

public interface IAssemblyDescriptor {
    string Name { get; }
    string FullName { get; }
    Version Version { get; }
    string RuntimeVersion { get; }
    CultureInfo? CultureInfo { get; }
    string? PublicKey { get; }
    string? PublicKeyToken { get; }
    string Location { get; }
    MethodInfo? EntryPoint { get; }
    Module ManifestModule { get; }
    IEnumerable<Attribute> GetCustomAttributes();
    IEnumerable<Attribute> GetCustomAttributes(Type attributeType);
    IEnumerable<Attribute> GetCustomAttributes<TAttribute>()
        where TAttribute : Attribute;
    TAttribute? GetCustomAttribute<TAttribute>()
        where TAttribute : Attribute;
    Attribute? GetCustomAttribute(Type attributeType);
    IEnumerable<TypeInfo> DefinedTypes { get; }
    IEnumerable<Type> ExportedTypes { get; }
    IEnumerable<Module> Modules { get; }
}
