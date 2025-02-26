namespace DotNetToolbox.Data.Repositories;

public partial class InMemoryDataSourceTests {
    [Fact]
    public void Create_BaseStrategy_ShouldThrow() {
        var action = () => _dummyDataSource.Create(_ => { });
        action.Should().Throw<NotImplementedException>();
    }

    [Fact]
    public async Task CreateAsync_WithValidationContext_ShouldThrow() {
        var action = () => _dummyDataSource.CreateAsync((_, _) => Task.CompletedTask, Map.Empty());
        await action.Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public async Task CreateAsync_BaseStrategy_ShouldThrow() {
        var action = () => _dummyDataSource.CreateAsync((_, _) => Task.CompletedTask);
        await action.Should().ThrowAsync<NotImplementedException>();
    }

    [Fact]
    public void Create_CreatesItem() {
        var result = _updatableRepo.Create(s => s.Name = "Z");
        result.IsSuccessful.Should().BeTrue();
        result.Value.Name.Should().Be("Z");
    }

    [Fact]
    public async Task CreateAsync_CreatesItem() {
        var result = await _updatableRepo.CreateAsync((s, _) => {
            s.Name = "Z";
            return Task.CompletedTask;
        });
        result.Should().BeOfType<TestEntity>();
    }

    [Fact]
    public async Task CreateAsync_WithValidationContext_CreatesItem() {
        var result = await _updatableRepo.CreateAsync((s, _) => {
            s.Name = "Z";
            return Task.CompletedTask;
        }, Map.Empty());
        result.IsSuccessful.Should().BeTrue();
        result.Value.Name.Should().Be("Z");
    }
}
