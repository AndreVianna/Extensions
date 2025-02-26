using DotNetToolbox.Options;

namespace DotNetToolbox.Singleton;

public class NamedOptionsTests {
    private sealed class TestOptions : NamedOptions<TestOptions>;
    private sealed class SomeObject : NamedOptions<SomeObject>;

    [Fact]
    public void Static_Default_ReturnsSingleton() {
        // Arrange & Act
        var instance1 = TestOptions.Default;
        var instance2 = TestOptions.Default;

        // Assert
        instance1.Should().BeOfType<TestOptions>();
        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void SectionName_ReturnsName() {
        // Arrange & Act & Assert
        TestOptions.SectionName.Should().Be("Test");
        SomeObject.SectionName.Should().Be("SomeObject");
    }
}
