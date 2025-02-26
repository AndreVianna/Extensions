namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for Assembly functionality.")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global - Used for externally.
public class AssemblyAccessor
    : IHasDefault<AssemblyAccessor>
    , IAssemblyAccessor {
    public static AssemblyAccessor Default { get; } = new();

    public virtual IAssemblyDescriptor GetExecutingAssembly()
        => new AssemblyDescriptor(Assembly.GetExecutingAssembly());

    public virtual IAssemblyDescriptor? GetEntryAssembly() {
        var assembly = Assembly.GetEntryAssembly();
        return assembly is null ? null : new AssemblyDescriptor(assembly);
    }

    public virtual IAssemblyDescriptor? GetDeclaringAssembly(Type type) {
        var assembly = Assembly.GetAssembly(type);
        return assembly is null ? null : new AssemblyDescriptor(assembly);
    }

    public virtual IAssemblyDescriptor? GetDeclaringAssembly<TType>()
        => GetDeclaringAssembly(typeof(TType));

    public virtual IAssemblyDescriptor GetCallingAssembly()
        => new AssemblyDescriptor(Assembly.GetCallingAssembly());
}
