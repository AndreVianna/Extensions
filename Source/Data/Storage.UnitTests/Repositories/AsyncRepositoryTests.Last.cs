namespace DotNetToolbox.Data.Repositories;

public partial class AsyncRepositoryTests {
    [Fact]
    public async Task LastAsync_ForPopulatedSet_ReturnsLastElement() {
        var expectedItem = new TestEntity("C");
        var result = await _repo.LastAsync();
        result.Should().Be(expectedItem);
    }

    [Fact]
    public async Task LastAsync_ForEmptySet_Throws() {
        var result = async () => await _emptyRepo.LastAsync();
        await result.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task LastAsync_WithValidPredicate_ReturnsElement() {
        var expectedItem = new TestEntity("B");
        var result = await _repo.LastAsync(x => x.Name == "B");
        result.Should().Be(expectedItem);
    }

    [Fact]
    public async Task LastAsync_WithInvalidPredicate_ReturnsElement() {
        var result = async () => await _emptyRepo.LastAsync(x => x.Name == "K");
        await result.Should().ThrowAsync<InvalidOperationException>();
    }
}
