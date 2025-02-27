using DotNetToolbox.Environment;

namespace DotNetToolbox.DependencyInjection;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void AddEnvironment_RegisterHandlers() {
        var services = new ServiceCollection();
        var result = services.SetEnvironment();
        using var provider = result.BuildServiceProvider();
        var environment = provider.GetRequiredService<IApplicationEnvironment>();

        environment.Name.Should().BeEmpty();
        environment.Assembly.Should().NotBeNull();
        environment.OperatingSystem.DateTime.Should().NotBeNull();
        environment.OperatingSystem.Output.Should().NotBeNull();
        environment.OperatingSystem.Input.Should().NotBeNull();
        environment.OperatingSystem.FileSystem.Should().NotBeNull();

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddEnvironment_WithNamedEnvironment_RegisterHandlers() {
        var services = new ServiceCollection();
        var result = services.SetEnvironment("Development");
        var provider = services.BuildServiceProvider();
        var environment = provider.GetRequiredService<IApplicationEnvironment>();

        environment.Name.Should().Be("Development");
        environment.Assembly.Should().NotBeNull();
        environment.OperatingSystem.DateTime.Should().NotBeNull();
        environment.OperatingSystem.Output.Should().NotBeNull();
        environment.OperatingSystem.Input.Should().NotBeNull();
        environment.OperatingSystem.FileSystem.Should().NotBeNull();

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void SetEnvironment_WithFakeServices_RegisterHandlers() {
        var services = new ServiceCollection();
        var assembly = Substitute.For<IAssemblyDescriptor>();
        var dateTime = Substitute.For<IDateTimeProvider>();
        var guid = Substitute.For<IGuidProvider>();
        var fileSystem = Substitute.For<IFileSystemAccessor>();
        var input = Substitute.For<IInput>();
        var output = Substitute.For<IOutput>();
        services.SetEnvironment("Development",
                                assembly,
                                dateTime,
                                guid,
                                fileSystem,
                                input,
                                output);
        var provider = services.BuildServiceProvider();

        var env = provider.GetRequiredService<IApplicationEnvironment>();

        env.Name.Should().Be("Development");
        env.Assembly.Should().NotBeNull();
        env.OperatingSystem.Should().NotBeNull();
        env.OperatingSystem.Name.Should().NotBeNull();
        env.OperatingSystem.DateTime.Should().NotBeNull();
        env.OperatingSystem.Guid.Should().NotBeNull();
        env.OperatingSystem.Output.Should().NotBeNull();
        env.OperatingSystem.Input.Should().NotBeNull();
        env.OperatingSystem.FileSystem.Should().NotBeNull();
    }
}
