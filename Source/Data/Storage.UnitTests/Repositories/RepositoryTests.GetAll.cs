namespace DotNetToolbox.Data.Repositories;

public partial class RepositoryTests {
    [Fact]
    public void GetAll_BaseStrategy_ShouldThrow() {
        var action = () => _dummyRepository.GetAll();
        action.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public async Task GetAllAsync_BaseStrategy_ShouldThrow() {
        var action = async () => await _dummyRepository.GetAllAsync();
        await action.Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public void GetAll_GetAllsItem() {
        var result = _updatableRepo.GetAll();
        result.Should().BeOfType<TestEntity[]>();
        result.Count().Should().Be(3);
    }

    [Fact]
    public async Task GetAllAsync_GetAllsItem() {
        var result = await _updatableRepo.GetAllAsync();
        result.Should().BeOfType<TestEntity[]>();
        result.Count().Should().Be(3);
    }
}
