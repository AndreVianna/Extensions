namespace System.Linq.Async;

public partial class AsyncQueryableTests {
    [Fact]
    public void ToLookup_WithMapping_ReturnsMappedLookup() {
        var result = _repo.ToLookup(x => x.Name);
        result.Should().HaveCount(3);
        result.First().Key.Should().Be("A");
        result["A"].Should().BeEquivalentTo([ new TestEntity("A") ]);
    }

    [Fact]
    public void ToLookup_WithMapping_ReturnsLookup() {
        var result = _repo.ToLookup(x => x.Name, x => $"{x.Name}*");
        result.Should().HaveCount(3);
        result.First().Key.Should().Be("A");
        result["A"].Should().BeEquivalentTo("A*");
    }

    [Fact]
    public void ToLookup_WithComparer_ReturnsLookup() {
        var result = _repo.ToLookup(x => x.Name, EqualityComparer<string>.Default);
        result.Should().HaveCount(3);
        result.First().Key.Should().Be("A");
        result["A"].Should().BeEquivalentTo([new TestEntity("A")]);
    }

    [Fact]
    public void ToLookup_WithMappingAndComparer_ReturnsLookup() {
        var result = _repo.ToLookup(x => x.Name, x => $"{x.Name}*", EqualityComparer<string>.Default);
        result.Should().HaveCount(3);
        result.First().Key.Should().Be("A");
        result["A"].Should().BeEquivalentTo("A*");
    }
}
