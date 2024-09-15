namespace DotNetToolbox.Data.Repositories;

public partial class InMemoryDataSourceTests {
    [Fact]
    public void GetAll_BaseStrategy_ShouldThrow() {
        var action = () => _dummyDataSource.GetAll();
        action.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public async Task GetAllAsync_BaseStrategy_ShouldThrow() {
        var action = async () => await _dummyDataSource.GetAllAsync();
        await action.Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public void GetAll_GetAllItems() {
        var result = _updatableRepo.GetAll();
        result.Should().BeOfType<TestEntity[]>();
        result.Length.Should().Be(3);
    }

    [Fact]
    public async Task GetAllAsync_GetAllItems() {
        var result = await _updatableRepo.GetAllAsync();
        result.Should().BeOfType<TestEntity[]>();
        result.Length.Should().Be(3);
    }
}