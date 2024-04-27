namespace DotNetToolbox.Data.Repositories;

public partial class AsyncRepositoryTests {
    [Fact]
    public async Task FirstAsync_ForPopulatedSet_ReturnsFirstElement() {
        var expectedItem = new TestEntity("A");
        var result = await _repo.FirstAsync();
        result.Should().Be(expectedItem);
    }

    [Fact]
    public async Task FirstAsync_ForEmptySet_Throws() {
        var result = async () => await _emptyRepo.FirstAsync();
        await result.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task FirstAsync_WithValidPredicate_ReturnsElement() {
        var expectedItem = new TestEntity("B");
        var result = await _repo.FirstAsync(x => x.Name == "B");
        result.Should().Be(expectedItem);
    }

    [Fact]
    public async Task FirstAsync_WithInvalidPredicate_ReturnsElement() {
        var result = async () => await _emptyRepo.FirstAsync(x => x.Name == "K");
        await result.Should().ThrowAsync<InvalidOperationException>();
    }
}
