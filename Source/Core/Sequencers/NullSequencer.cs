namespace DotNetToolbox.Sequencers;

public class NullSequencer<TKey>
    : IHasDefault<NullSequencer<TKey>>
    , ISequencer<TKey>
    where TKey : notnull {
    public static NullSequencer<TKey> Default => new();

    public TKey First => default!;
    public TKey Current {
        get => default!;
        set { }
    }
    TKey IEnumerator<TKey>.Current => Current;
    object IEnumerator.Current => Current;

    public void Dispose() => GC.SuppressFinalize(this);
    public bool MoveNext() => false;
    public void Reset() { }
    public void Set(TKey value, bool skip = false) { }
}
