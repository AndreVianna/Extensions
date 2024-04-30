namespace DotNetToolbox.Data.Repositories;

public partial class RepositoryTests {
    [Fact]
    public async Task LoadAsync_ForEmptySet_ReturnsZero() {
        var action = () => _repo.LoadAsync();
        await action.Should().NotThrowAsync();
    }
}