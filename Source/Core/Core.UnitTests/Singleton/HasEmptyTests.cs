using DotNetToolbox.Singletons;

namespace DotNetToolbox.Singleton;

public class HasEmptyTests {
    private sealed class ClassWithEmpty
        : IHasEmpty<ClassWithEmpty> {
        public static ClassWithEmpty Empty { get; } = new();
    }

    [Fact]
    public void Static_Empty_ReturnsSingleton() {
        // Arrange & Act
        var instance1 = ClassWithEmpty.Empty;
        var instance2 = ClassWithEmpty.Empty;

        // Assert
        instance1.Should().BeOfType<ClassWithEmpty>();
        instance1.Should().BeSameAs(instance2);
    }
}
