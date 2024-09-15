﻿namespace DotNetToolbox.Data.File;

public sealed class JsonFilePerRecordStorageTests : IDisposable {
    private readonly IConfigurationRoot _configuration;
    private readonly string _testDirectory;
    private readonly string _baseFolder;

    public JsonFilePerRecordStorageTests() {
        // Set up test directory
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);

        // Set up configuration
        var inMemorySettings = new Dictionary<string, string> {
            ["Data:BaseFolder"] = _testDirectory,
        };
#pragma warning disable CS8620
        _configuration = new ConfigurationBuilder()
                        .AddInMemoryCollection(inMemorySettings)
                        .Build();
#pragma warning restore CS8620

        _baseFolder = Path.Combine(_testDirectory, "TestData");
    }

    // Dispose the test directory after tests
    public void Dispose() {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, true);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly() {
        // Arrange & Act
        var storage = new TestJsonStorage("TestData", _configuration);

        // Assert
        storage.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WhenFolderDoesNotExist_CreatesEmptyFolder() {
        // Arrange & Act
        _ = new TestJsonStorage("TestData", _configuration);

        // Assert
        Directory.Exists(_baseFolder).Should().BeTrue();
        var files = Directory.EnumerateFiles(_baseFolder);
        files.Should().HaveCount(0);
    }

    [Fact]
    public void Constructor_WhenFolderExists_DoesNotOverwriteFile() {
        // Arrange
        Directory.CreateDirectory(_baseFolder);
        System.IO.File.WriteAllText($"{_baseFolder}\\1.json", "{\"Key\":1,\"Name\":\"TestItem\",\"Value\":10}");

        // Act
        _ = new TestJsonStorage("TestData", _configuration);

        // Assert
        var files = Directory.EnumerateFiles(_baseFolder).ToArray();
        files.Should().BeEquivalentTo($"{_baseFolder}\\1.json");
    }

    [Fact]
    public void Constructor_WhenBaseFolderNotInConfig_UsesDefaultBaseFolder() {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();
        var expectedPath = Path.Combine("data", "TestData");

        // Act
        var storage = new TestJsonStorage("TestData", emptyConfig);

        // Assert
        storage.BaseFolderPath.Should().Be(expectedPath);
    }

    [Fact]
    public void Load_WhenFileHasData_LoadsDataIntoRepository() {
        // Arrange
        Directory.CreateDirectory(_baseFolder);
        System.IO.File.WriteAllText($"{_baseFolder}\\1.json", "{\"Key\":1,\"Name\":\"Item1\",\"Value\":10}");
        System.IO.File.WriteAllText($"{_baseFolder}\\2.json", "{\"Key\":2,\"Name\":\"Item2\",\"Value\":7}");

        var storage = new TestJsonStorage("TestData", _configuration);

        // Act
        var result = storage.Load();

        // Assert
        result.IsSuccess.Should().BeTrue();
        storage.Data.Should().HaveCount(2);
        storage.Data.Should().ContainSingle(i => i.Key == 1 && i.Name == "Item1");
        storage.Data.Should().ContainSingle(i => i.Key == 2 && i.Name == "Item2");
    }

    [Fact]
    public void Load_WhenFileContainsInvalidJson_ThrowsJsonException() {
        // Arrange
        Directory.CreateDirectory(_baseFolder);
        System.IO.File.WriteAllText($"{_baseFolder}\\1.json", "Invalid JSON Content");
        var storage = new TestJsonStorage("TestData", _configuration);

        // Act
        Action act = () => storage.Load();

        // Assert
        act.Should().Throw<JsonException>();
    }
    [Fact]
    public void LoadLastUsedKey_WhenRepositoryHasData_SetsLastUsedKey() {
        // Arrange
        Directory.CreateDirectory(_baseFolder);
        System.IO.File.WriteAllText($"{_baseFolder}\\1.json", "{\"Key\":1,\"Name\":\"Item1\",\"Value\":10}");
        System.IO.File.WriteAllText($"{_baseFolder}\\3.json", "{\"Key\":3,\"Name\":\"Item3\",\"Value\":7}");
        System.IO.File.WriteAllText($"{_baseFolder}\\2.json", "{\"Key\":2,\"Name\":\"Item2\",\"Value\":5}");
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();

        // Act
        var result = storage.CallLoadLastUsedKey();

        // Assert
        result.IsSuccess.Should().BeTrue();
        storage.GetLastUsedKey.Should().Be(3);
    }

    [Fact]
    public void LoadLastUsedKey_WhenRepositoryIsEmpty_LastUsedKeyIsDefault() {
        // Arrange
        Directory.CreateDirectory(_baseFolder);
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();

        // Act
        var result = storage.CallLoadLastUsedKey();

        // Assert
        result.IsSuccess.Should().BeTrue();
        storage.GetLastUsedKey.Should().Be(default(int));
    }

    [Fact]
    public void Create_WithValidItem_AssignsNextKey() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "ExistingItem" });

        // Act
        var result = storage.Create(item => item.Name = "NewItem");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Key.Should().Be(0);
        result.Value.Name.Should().Be("NewItem");
    }

    [Fact]
    public void Create_WhenSetItemIsNull_CreatesItemWithDefaultValues() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);

        // Act
        var result = storage.Create();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("Name is required."));
    }

    [Fact]
    public void Add_WithValidItem_SavesToRepositoryAndFile() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();

        var newItem = new TestItem { Name = "NewItem" };

        // Act
        var result = storage.Add(newItem);

        // Assert
        result.IsSuccess.Should().BeTrue();
        storage.Data.Should().ContainSingle(i => i.Name == "NewItem");
        var filePath = Path.Combine(_baseFolder, $"{newItem.Key}.json");
        var content = System.IO.File.ReadAllText(filePath);
        content.Should().Contain("\"Name\": \"NewItem\"");
    }

    [Fact]
    public void Add_WhenItemIsInvalid_ReturnsValidationErrors() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        var invalidItem = new TestItem(); // Assuming Name is required

        // Act
        var result = storage.Add(invalidItem);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("Name is required."));
    }

    [Fact]
    public void GetAll_WithNoFilterOrSort_ReturnsAllItems() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "Item1" });
        storage.Add(new() { Key = 2, Name = "Item2" });

        // Act
        var items = storage.GetAll();

        // Assert
        items.Should().HaveCount(2);
    }

    [Fact]
    public void GetAll_WithFilter_ReturnsFilteredItems() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "Apple" });
        storage.Add(new() { Key = 2, Name = "Banana" });

        // Act
        var items = storage.GetAll(item => item.Name.StartsWith('A'));

        // Assert
        items.Should().HaveCount(1);
        items[0].Name.Should().Be("Apple");
    }

    [Fact]
    public void GetAll_WithSorting_ReturnsSortedItems() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "Charlie" });
        storage.Add(new() { Key = 2, Name = "Bravo" });
        storage.Add(new() { Key = 3, Name = "Alpha" });

        var sortClauses = new HashSet<SortClause> {
            new SortClause<string>("Name", SortDirection.Ascending),
        };

        // Act
        var items = storage.GetAll(orderBy: sortClauses);

        // Assert
        items.Should().HaveCount(3);
        items[0].Name.Should().Be("Alpha");
        items[1].Name.Should().Be("Bravo");
        items[2].Name.Should().Be("Charlie");
    }

    [Fact]
    public void GetAll_WithEmptySorting_ReturnsSortedItems() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "Charlie" });
        storage.Add(new() { Key = 2, Name = "Bravo" });
        storage.Add(new() { Key = 3, Name = "Alpha" });

        var sortClauses = new HashSet<SortClause>();

        // Act
        var items = storage.GetAll(orderBy: sortClauses);

        // Assert
        items.Should().HaveCount(3);
        items[0].Name.Should().Be("Charlie");
        items[1].Name.Should().Be("Bravo");
        items[2].Name.Should().Be("Alpha");
    }

    [Fact]
    public void GetAll_WithInvalidSortProperty_ThrowsArgumentException() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "Item1" });

        var sortClauses = new HashSet<SortClause> {
            new SortClause<string>("InvalidProperty", SortDirection.Ascending),
        };

        // Act
        Action act = () => storage.GetAll(orderBy: sortClauses);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("Property InvalidProperty not found on TestItem.*")
           .And.ParamName.Should().Be("orderBy");
    }

    [Fact]
    public void GetAll_WithMultipleSortClauses_ReturnsItemsSorted() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "ItemA", Value = 5 });
        storage.Add(new() { Key = 2, Name = "ItemA", Value = 10 });
        storage.Add(new() { Key = 3, Name = "ItemB", Value = 7 });
        var orderBy = new HashSet<SortClause> {
            new SortClause<string>("Name", SortDirection.Ascending),
            new SortClause<int>("Value", SortDirection.Descending),
        };

        // Act
        var result = storage.GetAll(orderBy: orderBy);

        // Assert
        result.Select(i => i.Key).Should().ContainInOrder(2, 1, 3);
    }

    [Fact]
    public void GetAll_WithComplexSortClauses_ReturnsItemsSorted() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "ItemA", Value = 1 });
        storage.Add(new() { Key = 2, Name = "ItemA", Value = 3 });
        storage.Add(new() { Key = 3, Name = "ItemB", Value = 2 });
        storage.Add(new() { Key = 4, Name = "ItemB", Value = 1 });
        storage.Add(new() { Key = 5, Name = "ItemA", Value = 2 });
        storage.Add(new() { Key = 6, Name = "ItemB", Value = 3 });
        var orderBy = new HashSet<SortClause> {
            new SortClause<string>("Name", SortDirection.Ascending),
            new SortClause<int>("Value", SortDirection.Descending),
        };

        // Act
        var result = storage.GetAll(orderBy: orderBy);

        // Assert
        result.Select(i => i.Key).Should().ContainInOrder(2, 5, 1, 6, 3, 4);
    }

    [Fact]
    public void GetAll_WithInvertedSortClauses_ReturnsItemsSorted() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "ItemA", Value = 1 });
        storage.Add(new() { Key = 2, Name = "ItemA", Value = 3 });
        storage.Add(new() { Key = 3, Name = "ItemB", Value = 2 });
        storage.Add(new() { Key = 4, Name = "ItemB", Value = 1 });
        storage.Add(new() { Key = 5, Name = "ItemA", Value = 2 });
        storage.Add(new() { Key = 6, Name = "ItemB", Value = 3 });
        var orderBy = new HashSet<SortClause> {
            new SortClause<string>("Name", SortDirection.Descending),
            new SortClause<int>("Value", SortDirection.Ascending),
        };

        // Act
        var result = storage.GetAll(orderBy: orderBy);

        // Assert
        result.Select(i => i.Key).Should().ContainInOrder(4, 3, 6, 1, 5, 2);
    }

    [Fact]
    public void Find_WhenItemExists_ReturnsItem() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        var testItem = new TestItem { Key = 1, Name = "Item1" };
        storage.Add(testItem);

        // Act
        var result = storage.Find(item => item.Name == "Item1");

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testItem);
    }

    [Fact]
    public void Find_WhenItemDoesNotExist_ReturnsNull() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();

        // Act
        var result = storage.Find(item => item.Name == "NonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindByKey_WithExistingKey_ReturnsItem() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        var existingItem = new TestItem { Key = 1, Name = "ExistingItem" };
        storage.Add(existingItem);

        // Act
        var item = storage.FindByKey(1);

        // Assert
        item.Should().NotBeNull();
        item.Should().BeEquivalentTo(existingItem);
    }

    [Fact]
    public void FindByKey_WithNonExistingKey_ReturnsNull() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();

        // Act
        var item = storage.FindByKey(99);

        // Assert
        item.Should().BeNull();
    }

    [Fact]
    public void Update_WithExistingItem_UpdatesItem() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        var existingItem = new TestItem { Key = 1, Name = "OldName" };
        storage.Add(existingItem);

        var updatedItem = new TestItem { Key = 1, Name = "NewName" };

        // Act
        var result = storage.Update(updatedItem);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var item = storage.FindByKey(1);
        item.Should().NotBeNull();
        item.Name.Should().Be("NewName");
    }

    [Fact]
    public void Update_WithNonExistingItem_ReturnsError() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        var updatedItem = new TestItem { Key = 99, Name = "NewName" };

        // Act
        var result = storage.Update(updatedItem);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Message == "Item '99' not found");
    }

    [Fact]
    public void Update_WhenItemIsInvalid_ReturnsValidationErrors() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        var existingItem = new TestItem { Key = 1, Name = "ValidName" };
        storage.Add(existingItem);
        var invalidItem = new TestItem { Key = 1, Name = "" }; // Invalid Name

        // Act
        var result = storage.Update(invalidItem);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Message.Contains("Name is required"));
    }

    [Fact]
    public void Remove_WithExistingKey_RemovesItem() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Add(new() { Key = 1, Name = "ItemToRemove" });

        // Act
        var result = storage.Remove(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        storage.Data.Should().BeEmpty();
        System.IO.File.Exists($"{_baseFolder}\\1.json").Should().BeFalse();
    }

    [Fact]
    public void Remove_WithExistingKey_ButNoFile_RemovesItem() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();
        storage.Data.Add(new() { Key = 1, Name = "ItemToRemove" });

        // Act
        var result = storage.Remove(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        storage.Data.Should().BeEmpty();
        System.IO.File.Exists($"{_baseFolder}\\1.json").Should().BeFalse();
    }

    [Fact]
    public void Remove_WithNonExistingKey_ReturnsError() {
        // Arrange
        var storage = new TestJsonStorage("TestData", _configuration);
        storage.Load();

        // Act
        var result = storage.Remove(99);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Message == "Item '99' not found");
    }

    private sealed class TestItem : IEntity<uint> {
        public uint Key { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }

        public Result Validate(IMap? context = null) {
            var result = Result.Success();
            if (string.IsNullOrEmpty(Name)) result += Result.Invalid("Name is required.");
            return result;
        }
    }

    private sealed class TestJsonStorage(string name, IConfiguration configuration)
        : JsonFilePerRecordStorage<TestItem, uint>(name, configuration, []) {
        public uint GetLastUsedKey => LastUsedKey;

        public Result<uint> CallLoadLastUsedKey() => LoadLastUsedKey();

        protected override uint FirstKey => 1;

        protected override bool TryGenerateNextKey(out uint next) {
            next = LastUsedKey == default ? FirstKey : ++LastUsedKey;
            return true;
        }
    }
}