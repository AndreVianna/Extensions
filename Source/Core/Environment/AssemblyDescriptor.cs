namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for Assembly functionality.")]
public class AssemblyDescriptor
    : IHasDefault<AssemblyDescriptor>
    , IAssemblyDescriptor {
    public static AssemblyDescriptor Default { get; } = new();

    private readonly Assembly _assembly;

    public AssemblyDescriptor() {
        _assembly = Assembly.GetEntryAssembly()!;
    }

    internal AssemblyDescriptor(Assembly assembly) {
        _assembly = assembly;
    }

    public virtual string Name => _assembly.GetName().Name!;
    public virtual string FullName => _assembly.FullName!;
    public virtual Version Version => _assembly.GetName().Version!;
    public virtual CultureInfo? CultureInfo => _assembly.GetName().CultureInfo;
    public virtual string? PublicKey {
        get {
            var bytes = _assembly.GetName().GetPublicKey();
            return bytes is null ? null : Encoding.UTF8.GetString(bytes);
        }
    }
    public virtual string? PublicKeyToken {
        get {
            var bytes = _assembly.GetName().GetPublicKeyToken();
            return bytes is null ? null : Encoding.UTF8.GetString(bytes);
        }
    }

    public virtual string RuntimeVersion => _assembly.ImageRuntimeVersion;
    public virtual string Location => _assembly.Location;
    public virtual MethodInfo? EntryPoint => _assembly.EntryPoint;
    public virtual Module ManifestModule => _assembly.ManifestModule;
    public virtual IEnumerable<Attribute> GetCustomAttributes()
        => _assembly.GetCustomAttributes();
    public virtual IEnumerable<Attribute> GetCustomAttributes(Type attributeType)
        => _assembly.GetCustomAttributes(attributeType);
    public virtual IEnumerable<Attribute> GetCustomAttributes<TAttribute>()
        where TAttribute : Attribute
        => _assembly.GetCustomAttributes<TAttribute>();
    public virtual TAttribute? GetCustomAttribute<TAttribute>()
        where TAttribute : Attribute
        => _assembly.GetCustomAttribute<TAttribute>();
    public virtual Attribute? GetCustomAttribute(Type attributeType)
        => _assembly.GetCustomAttribute(attributeType);
    public virtual IEnumerable<TypeInfo> DefinedTypes => _assembly.DefinedTypes;
    public virtual IEnumerable<Type> ExportedTypes => _assembly.ExportedTypes;
    public virtual IEnumerable<Module> Modules => _assembly.Modules;
}
