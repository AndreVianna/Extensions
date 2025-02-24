namespace DotNetToolbox.Singletons;

public class HasEmptyTests {
    private sealed class ClassWithEmpty : HasEmpty<ClassWithEmpty>;

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
