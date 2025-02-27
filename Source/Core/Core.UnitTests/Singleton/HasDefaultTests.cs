using DotNetToolbox.Singletons;

namespace DotNetToolbox.Singleton;

public class HasDefaultTests {
    private sealed class ClassHasDefault
        : IHasDefault<ClassHasDefault> {
        public static ClassHasDefault Default => new();
    }

    [Fact]
    public void Static_Default_ReturnsSingleton() {
        // Arrange & Act
        var instance1 = ClassHasDefault.Default;
        var instance2 = ClassHasDefault.Default;

        // Assert
        instance1.Should().BeOfType<ClassHasDefault>();
        instance1.Should().BeSameAs(instance2);
    }
}
