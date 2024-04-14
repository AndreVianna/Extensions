namespace DotNetToolbox.Data.Repositories;

public class ReadOnlyRepository<TItem>
    : IReadOnlyRepository<TItem> {
    private readonly IQueryable<TItem> _data;

    public ReadOnlyRepository(IStrategyProvider? provider = null)
        : this([], provider) {
    }

    public ReadOnlyRepository(IEnumerable<TItem> source, IStrategyProvider? provider = null) {
        var list = IsNotNull(source).ToList();
        _data = list.AsQueryable();
        Strategy = (IRepositoryStrategy<TItem>?)provider?.GetStrategy<TItem>()
            ?? new InMemoryRepositoryStrategy<TItem>(list);
    }

    public Type ElementType => _data.ElementType;
    public Expression Expression => _data.Expression;
    public IQueryProvider Provider => _data.Provider;

    public IRepositoryStrategy<TItem> Strategy { get; }

    public IEnumerator<TItem> GetEnumerator() => _data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool HaveAny()
        => Strategy.HaveAny();
    public int Count()
        => Strategy.Count();
    public TItem[] ToArray()
        => Strategy.ToArray();
    public TItem? GetFirst()
        => Strategy.GetFirst();
}