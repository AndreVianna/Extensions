namespace DotNetToolbox;

public class MapTests {
    [Fact]
    public void Constructor_WithNullSource_CreatesEmptyMap() {
        // Act
        var map = new Map();

        // Assert
        map.Should().NotBeNull();
        map.Count.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithNonNullSource_CreatesMapWithSourceItems() {
        // Arrange
        var source = new Dictionary<string, object> { { "key1", "value1" }, { "key2", 2 } };

        // Act
        var map = new Map(source);

        // Assert
        map.Should().NotBeNull();
        map.Count.Should().Be(2);
        map["key1"].Should().Be("value1");
        map["key2"].Should().Be(2);
    }

    [Fact]
    public void GetValueAs_WithValidKeyAndCorrectType_ReturnsValue() {
        // Arrange
        var map = new Map(new Dictionary<string, object> { { "key", 123 } });

        // Act
        var value = map.GetValueOrDefault("key", 0);

        // Assert
        value.Should().Be(123);
    }

    [Fact]
    public void GetValueAs_WithValidKeyAndIncorrectType_ThrowsInvalidCastException() {
        // Arrange
        var map = new Map(new Dictionary<string, object> { { "key", 123 } });

        // Act
        Action action = () => map.GetValueAs<string>("key");

        // Assert
        action.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void GetValueAs_WithNullValueAndClassType_ReturnsDefault() {
        // Arrange
        var map = new Map(new Dictionary<string, object> { { "key", null! } });

        // Act
        var value = map.GetValueAs<string>("key");

        // Assert
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetValueAs_WithValidKeyAndCorrectType_ReturnsTrueAndValue() {
        // Arrange
        var map = new Map(new Dictionary<string, object> { { "key", 123 } });

        // Act
        var result = map.TryGetValueAs<int>("key", out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(123);
    }

    [Fact]
    public void TryGetValueAs_WithValidKeyAndIncorrectType_ReturnsFalse() {
        // Arrange
        var map = new Map(new Dictionary<string, object> { { "key", 123 } });

        // Act
        var result = map.TryGetValueAs<string>("key", out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetValueAs_WithMissingKey_ReturnsFalse() {
        // Arrange
        var map = new Map();

        // Act
        var result = map.TryGetValueAs<int>("key", out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(0);
    }

    [Fact]
    public void Indexer_GetAndSet_WorksCorrectly() {
        // Arrange
#pragma warning disable IDE0028 // Simplify collection initialization
        var map = new Map();
#pragma warning restore IDE0028 // Simplify collection initialization

        // Act
        map["key"] = "value";
        var value = map["key"];

        // Assert
        value.Should().Be("value");
    }

    [Fact]
    public void Add_AddsItemToMap() {
        // Arrange
#pragma warning disable IDE0028 // Simplify collection initialization
        var map = new Map();
#pragma warning restore IDE0028 // Simplify collection initialization

        // Act
        map.Add("key", "value");

        // Assert
        map["key"].Should().Be("value");
    }

    [Fact]
    public void Remove_RemovesItemFromMap() {
        // Arrange
        var map = new Map(new Dictionary<string, object> { { "key", "value" } });

        // Act
        var result = map.Remove("key");

        // Assert
        result.Should().BeTrue();
        map.ContainsKey("key").Should().BeFalse();
    }

    [Fact]
    public void Clear_RemovesAllItemsFromMap() {
        // Arrange
        var map = new Map(new Dictionary<string, object> { { "key1", "value1" }, { "key2", 2 } });

        // Act
        map.Clear();

        // Assert
        map.Count.Should().Be(0);
    }

    [Fact]
    public void ContainsKey_ReturnsTrueIfKeyExists() {
        // Arrange
        var map = new Map(new Dictionary<string, object> { { "key", "value" } });

        // Act
        var result = map.ContainsKey("key");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TryGetValue_ReturnsTrueIfKeyExists() {
        // Arrange
        var map = new Map(new Dictionary<string, object> { { "key", "value" } });

        // Act
        var result = map.TryGetValue("key", out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be("value");
    }

    [Fact]
    public void Constructor_ForMapOfT_WithNullSource_CreatesEmptyMap() {
        // Act
        var map = new Map<int>();

        // Assert
        map.Should().NotBeNull();
        map.Count.Should().Be(0);
    }

    [Fact]
    public void Constructor_ForMapOfT_WithNonNullSource_CreatesMapWithSourceItems() {
        // Arrange
        var source = new Dictionary<string, int> { { "key1", 1 }, { "key2", 2 } };

        // Act
        var map = new Map<int>(source);

        // Assert
        map.Should().NotBeNull();
        map.Count.Should().Be(2);
        map["key1"].Should().Be(1);
        map["key2"].Should().Be(2);
    }

    [Fact]
    public void Indexer_ForMapOfT_GetAndSet_WorksCorrectly() {
        // Arrange
#pragma warning disable IDE0028 // Simplify collection initialization
        var map = new Map<int>();
#pragma warning restore IDE0028 // Simplify collection initialization

        // Act
        map["key"] = 1;
        var value = map["key"];

        // Assert
        value.Should().Be(1);
    }

    [Fact]
    public void Add_ForMapOfT_AddsItemToMap() {
        // Arrange
#pragma warning disable IDE0028 // Simplify collection initialization
        var map = new Map<int>();
#pragma warning restore IDE0028 // Simplify collection initialization

        // Act
        map.Add("key", 1);

        // Assert
        map["key"].Should().Be(1);
    }

    [Fact]
    public void Remove_ForMapOfT_RemovesItemFromMap() {
        // Arrange
        var map = new Map<int>(new Dictionary<string, int> { { "key", 1 } });

        // Act
        var result = map.Remove("key");

        // Assert
        result.Should().BeTrue();
        map.ContainsKey("key").Should().BeFalse();
    }

    [Fact]
    public void Clear_ForMapOfT_RemovesAllItemsFromMap() {
        // Arrange
        var map = new Map<int>(new Dictionary<string, int> { { "key1", 1 }, { "key2", 2 } });

        // Act
        map.Clear();

        // Assert
        map.Count.Should().Be(0);
    }

    [Fact]
    public void ContainsKey_ForMapOfT_ReturnsTrueIfKeyExists() {
        // Arrange
        var map = new Map<int>(new Dictionary<string, int> { { "key", 1 } });

        // Act
        var result = map.ContainsKey("key");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TryGetValue_ForMapOfT_ReturnsTrueIfKeyExists() {
        // Arrange
        var map = new Map<int>(new Dictionary<string, int> { { "key", 1 } });

        // Act
        var result = map.TryGetValue("key", out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(1);
    }

    [Fact]
    public void GetEnumerator_ForMapOfT_ReturnsAllItems() {
        // Arrange
        var source = new Dictionary<string, int> { { "key1", 1 }, { "key2", 2 } };
        var map = new Map<int>(source);

        // Act
        var items = map.ToList();

        // Assert
        items.Should().HaveCount(2);
        items.Should().Contain(new KeyValuePair<string, int>("key1", 1));
        items.Should().Contain(new KeyValuePair<string, int>("key2", 2));
    }
}
