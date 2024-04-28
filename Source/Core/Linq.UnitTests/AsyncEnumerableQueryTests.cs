namespace DotNetToolbox.Data.Repositories;

public class AsyncEnumerableQueryTests {
    [Fact]
    public void Query_ReturnsIQueryable() {
        var list = new[] { 1, 2, 3 };

        var query = list.ToAsyncQueryable();

        var subject = query.Should().BeOfType<AsyncEnumerableQuery>().Subject;
        subject.ElementType.Should().Be(typeof(TestEntity));
        subject.Expression.Should().NotBeNull();
        subject.Provider.Should().NotBeNull();
        subject.AsyncProvider.Should().NotBeNull();
    }
}