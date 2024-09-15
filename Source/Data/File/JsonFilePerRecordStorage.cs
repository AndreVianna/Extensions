namespace DotNetToolbox.Data.File;

public abstract class JsonFilePerRecordStorage<TItem, TKey>
    : Storage<JsonFilePerRecordStorage<TItem, TKey>, TItem, TKey>,
      IJsonFilePerRecordStorage<TItem, TKey>
    where TItem : class, IEntity<TKey>
    where TKey : notnull {
    private const string _defaultBaseFolder = "data";
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    protected JsonFilePerRecordStorage(string name, IConfiguration configuration, IList<TItem>? data = null)
        : base(data) {
        var baseFolder = configuration.GetValue<string>("Data:BaseFolder")
                      ?? configuration.GetValue<string>($"Data:{name}:BaseFolder")
                      ?? _defaultBaseFolder;
        BaseFolderPath = Path.Combine(baseFolder, name);
        EnsureFolderExists();
    }

    public string BaseFolderPath { get; }

    private void EnsureFolderExists()
     => Directory.CreateDirectory(BaseFolderPath);

    public override Result Load() {
        Data.Clear();
        foreach (var file in Directory.GetFiles(BaseFolderPath, "*.json")) {
            var json = System.IO.File.ReadAllText(file);
            var item = JsonSerializer.Deserialize<TItem>(json, _jsonOptions);
            if (item != null) Data.Add(item);
        }
        LoadLastUsedKey();
        return Result.Success();
    }

    protected override Result<TKey?> LoadLastUsedKey() {
        LastUsedKey = Data.Count != 0
            ? Data.Max(item => item.Key)
            : default;
        return Result.Success(LastUsedKey);
    }

    public override TItem[] GetAll(Expression<Func<TItem, bool>>? filterBy = null, HashSet<SortClause>? orderBy = null) {
        var query = Data.AsQueryable();
        query = ApplyFilter(query, filterBy);
        query = ApplySorting(query, orderBy);
        return [.. query];
    }

    private static IQueryable<TItem> ApplyFilter(IQueryable<TItem> query, Expression<Func<TItem, bool>>? filterBy = null)
     => filterBy is null ? query : query.Where(filterBy);

    private static IComparer<TItem> GetSortingComparer(PropertyInfo property)
        => Comparer<TItem>.Create((x, y) => {
            var xValue = property.GetValue(x);
            var yValue = property.GetValue(y);
            return Comparer.Default.Compare(xValue, yValue);
        });

    private static IQueryable<TItem> ApplySorting(IQueryable<TItem> query, HashSet<SortClause>? orderBy = null) {
        if (orderBy is null) return query;
        IOrderedQueryable<TItem>? orderedQuery = null;

        foreach (var clause in orderBy.Reverse()) {
            var property = typeof(TItem).GetProperty(clause.PropertyName)
                        ?? throw new ArgumentException($"Property {clause.PropertyName} not found on {typeof(TItem).Name}.", nameof(orderBy));
            orderedQuery = orderedQuery is null
                               ? clause.Direction == SortDirection.Ascending
                                     ? query.Order(GetSortingComparer(property))
                                     : query.OrderDescending(GetSortingComparer(property))
                               : clause.Direction == SortDirection.Ascending
                                   ? orderedQuery.Order(GetSortingComparer(property))
                                   : orderedQuery.OrderDescending(GetSortingComparer(property));
        }
        return orderedQuery ?? query;
    }

    public override TItem? FindByKey(TKey key)
        => Data.Find(item => item.Key.Equals(key));

    public override TItem? Find(Expression<Func<TItem, bool>> predicate)
        => Data.AsQueryable().FirstOrDefault(predicate);

    private string GetFilePath(TKey key) => Path.Combine(BaseFolderPath, $"{key}.json");

    private void SaveItem(TItem item) {
        var json = JsonSerializer.Serialize(item, _jsonOptions);
        var filePath = GetFilePath(item.Key);
        System.IO.File.WriteAllText(filePath, json);
    }

    public override Result<TItem> Create(Action<TItem>? setItem = null, IMap? validationContext = null) {
        var item = InstanceFactory.Create<TItem>();
        // Create does not consume a key to avoid consuming it into a record that might not be saved.
        setItem?.Invoke(item);
        var result = Result.Success(item);
        result += item.Validate(validationContext);
        return result;
    }

    public override Result Add(TItem newItem, IMap? validationContext = null) {
        if (TryGetNextKey(out var next)) newItem.Key = next;
        var result = newItem.Validate(validationContext);
        if (!result.IsSuccess) return result;
        Data.Add(newItem);
        SaveItem(newItem);
        return result;
    }

    public override Result Update(TItem updatedItem, IMap? validationContext = null) {
        var entry = Data.Index().FirstOrDefault(i => i.Item.Key.Equals(updatedItem.Key));
        if (entry.Item is null) return new ValidationError($"Item '{updatedItem.Key}' not found", nameof(updatedItem));
        var result = updatedItem.Validate(validationContext);
        if (!result.IsSuccess) return result;
        Data[entry.Index] = updatedItem;
        SaveItem(updatedItem);
        return result;
    }

    public override Result Remove(TKey key) {
        var entry = Data.Index().FirstOrDefault(i => i.Item.Key.Equals(key));
        if (entry.Item is null) return new ValidationError($"Item '{key}' not found", nameof(key));
        Data.RemoveAt(entry.Index);

        var filePath = GetFilePath(key);
        if (!System.IO.File.Exists(filePath)) return Result.Success();
        System.IO.File.Delete(filePath);
        return Result.Success();
    }
}
