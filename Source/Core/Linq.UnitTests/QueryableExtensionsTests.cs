// ReSharper disable once CheckNamespace - Intended to be in this namespace

using System.Text;

namespace System.Linq;

public class QueryableExtensionsTests {
    [Fact]
    public void ToIndexed_ForIQueryableOfT_ReturnsProjectedArray() {
        // Arrange
        var source = new List<int> { 1, 2, 3 }.AsQueryable();
        var expectedResult = new List<IndexedItem<int>> { new(0, 1, false), new(1, 2, false), new(2, 3, true) };

        // Act
        var result = source.ToIndexedList();

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ToIndexed_ForIQueryable_ReturnsProjectedArray() {
        // Arrange
        var source = new List<int> { 1, 2, 3 }.AsQueryable();
        var expectedResult = new List<IndexedItem<int>> { new(0, 1, false), new(1, 2, false), new(2, 3, true) };

        // Act
        var result = ((IQueryable)source).ToIndexedList<int>();

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ToArray_WithProject_ReturnsProjectedArray() {
        // Arrange
        Expression<Func<int, int>> project = x => x * 2;
        var source = new List<int> { 1, 2, 3 }.AsQueryable();

        // Act
        var result = source.ToArray<int>(project);

        // Assert
        result.Should().Equal(2, 4,
                              6);
    }

    [Fact]
    public void ToList_WithProject_ReturnsProjectedArray() {
        // Arrange
        Expression<Func<int, int>> project = x => x * 2;
        var source = new List<int> { 1, 2, 3 }.AsQueryable();

        // Act
        var result = source.ToList<int>(project);

        // Assert
        result.Should().Equal(2, 4,
                              6);
    }

    [Fact]
    public void ToDictionary_WithProjectAndSelectors_ReturnsProjectedDictionary() {
        // Arrange
        var source = new List<string> { "one", "two", "three" }.AsQueryable();

        // Act
        var result = source.ToDictionary(x => x.ToUpper(CultureInfo.InvariantCulture), x => x.Length);

        // Assert
        result.Should().Equal(new Dictionary<string, int> { { "ONE", 3 }, { "TWO", 3 }, { "THREE", 5 } });
    }

    [Fact]
    public void ToHashSet_WithProject_ReturnsProjectedHashSet() {
        // Arrange
        Expression<Func<int, int>> project = x => x * 2;
        var source = new List<int> { 1, 2, 3 }.AsQueryable();

        // Act
        var result = source.ToHashSet(project);

        // Assert
        result.Should().Equal(new HashSet<int> { 2, 4, 6 });
    }

    private sealed record TestEntity(string Name);

    [Fact]
    public void AsIndexed_ForIQueryableOfT_ReturnsIndexedQuery() {
        // Arrange
        var source = new List<TestEntity> {
            new("User1"),
            new("User2"),
        };

        var query = source.AsQueryable();

        // Act
        var result = query.AsIndexed();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<EnumerableQuery<Indexed<TestEntity>>>();
    }

    [Fact]
    public void AsIndexed_ForIQueryable_ReturnsIndexedQuery() {
        // Arrange
        var source = new List<TestEntity> {
            new("User1"),
            new("User2"),
        };

        var query = source.AsQueryable();

        // Act
        var result = ((IQueryable)query).AsIndexed<TestEntity>();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<EnumerableQuery<Indexed<TestEntity>>>();
    }

    [Fact]
    public void ForEach_ForIQueryable_LoopsThrowItems() {
        // Arrange
        var source = new List<TestEntity> {
            new("A"),
            new("B"),
            new("C"),
        };

        var query = source.AsQueryable();
        var result = new StringBuilder();

        // Act

        query.ForEach(x => result.Append(x.Name));

        // Assert
        result.ToString().Should().Be("ABC");
    }

    [Fact]
    public void ForEach_ForIQueryableOfT_LoopsThrowItems() {
        // Arrange
        var source = new List<TestEntity> {
            new("A"),
            new("B"),
            new("C"),
        };

        var query = source.AsQueryable();
        var result = new StringBuilder();

        // Act

        ((IQueryable)query).ForEach<TestEntity>(x => result.Append(x.Name));

        // Assert
        result.ToString().Should().Be("ABC");
    }
}
