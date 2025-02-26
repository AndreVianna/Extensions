namespace DotNetToolbox.Data.File;

public abstract class JsonFilePerTypeStorage<TItem>(string name, IConfiguration configuration, IList<TItem>? data = null)
    : JsonFilePerTypeStorage<TItem, uint>(name, configuration, data)
    where TItem : class, IEntity<uint> {
    protected override uint FirstKey { get; } = 1;

    protected override bool TryGenerateNextKey(out uint next) {
        next = LastUsedKey == default ? FirstKey : ++LastUsedKey;
        return true;
    }
}

public abstract class JsonFilePerTypeStorage<TItem, TKey>
    : Storage<JsonFilePerTypeStorage<TItem, TKey>, TItem, TKey>,
      IJsonFilePerTypeStorage<TItem, TKey>
    where TItem : class, IEntity<TKey>
    where TKey : notnull {
    private const string _defaultBaseFolder = "data";
    private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    protected JsonFilePerTypeStorage(string name, IConfiguration configuration, IList<TItem>? data = null)
        : base(data) {
        var baseFolder = configuration.GetValue<string>($"Data:{name}:BaseFolder");
        if (string.IsNullOrWhiteSpace(baseFolder)) baseFolder = configuration.GetValue<string>("Data:BaseFolder");
        if (string.IsNullOrWhiteSpace(baseFolder)) baseFolder = _defaultBaseFolder;
        FilePath = Path.Combine(baseFolder, $"{name}.json");
        EnsureFileExists(baseFolder);
    }

    public string FilePath { get; }

    private void EnsureFileExists(string baseFolder) {
        if (Path.Exists(FilePath)) return;
        Directory.CreateDirectory(baseFolder);
        Save();
    }

    public override Result Load() {
        Data.Clear();
        var json = System.IO.File.ReadAllText(FilePath);
        var items = JsonSerializer.Deserialize<TItem[]>(json, _jsonOptions)!;
        Data.AddRange(items);
        LoadLastUsedKey();
        return Result.Success();
    }

    protected override Result<TKey?> LoadLastUsedKey() {
        LastUsedKey = Data.Count != 0
            ? Data.Max(item => item.Id)
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
        => Data.Find(item => item.Id.Equals(key));

    public override TItem? Find(Expression<Func<TItem, bool>> predicate)
        => Data.AsQueryable().FirstOrDefault(predicate);

    private void Save() {
        var json = JsonSerializer.Serialize(Data, _jsonOptions);
        System.IO.File.WriteAllText(FilePath, json);
    }

    public override Result<TItem> Create(Action<TItem>? setItem = null, IMap? validationContext = null) {
        var item = InstanceFactory.Create<TItem>();
        // Create does not consume a key to avoid consuming it into a record that might not be saved.
        setItem?.Invoke(item);
        var result = Result.Success(item);
        result += item.Validate(validationContext);
        return result;
    }

    public override Result Add(TItem newItem, IMap? context = null) {
        context ??= new Map();
        context[nameof(EntityAction)] = EntityAction.Insert;
        var result = newItem.Validate(context);
        if (!result.IsSuccessful) return result;
        if (TryGetNextKey(out var next)) newItem.Id = next;
        Data.Add(newItem);
        Save();
        return result;
    }

    public override Result Update(TItem updatedItem, IMap? context = null) {
        context ??= new Map();
        context[nameof(EntityAction)] = EntityAction.Update;
        var entry = Data.Index().FirstOrDefault(i => i.Item.Id.Equals(updatedItem.Id));
        if (entry.Item is null) return new ValidationError($"Item '{updatedItem.Id}' not found", nameof(updatedItem));
        var result = updatedItem.Validate(context);
        if (!result.IsSuccessful) return result;
        Data[entry.Index] = updatedItem;
        Save();
        return result;
    }

    public override Result Remove(TKey key) {
        var entry = Data.Index().FirstOrDefault(i => i.Item.Id.Equals(key));
        if (entry.Item is null) return new ValidationError($"Item '{key}' not found", nameof(key));
        Data.RemoveAt(entry.Index);
        Save();
        return Result.Success();
    }
}
