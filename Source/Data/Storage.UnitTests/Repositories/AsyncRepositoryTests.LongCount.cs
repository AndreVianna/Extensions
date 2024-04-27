namespace DotNetToolbox.Data.Repositories;

public partial class AsyncRepositoryTests {
    [Fact]
    public async Task LongCountAsync_ForEmptySet_ReturnsZero() {
        var result = await _emptyRepo.LongCountAsync();
        result.Should().Be(0L);
    }

    [Fact]
    public async Task LongCount_ReturnsCount() {
        var result = await _repo.LongCountAsync();
        result.Should().Be(3L);
    }

    [Fact]
    public async Task LongCount_WithPredicate_ReturnsFilteredCount() {
        var result = await _repo.LongCountAsync(x => x.Name != "A");
        result.Should().Be(2L);
    }
}
