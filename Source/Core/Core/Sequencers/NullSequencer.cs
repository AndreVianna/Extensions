
namespace DotNetToolbox.Sequencers;

public class NullSequencer<TKey>
    : HasDefault<NullSequencer<TKey>>
    , ISequencer<TKey>
    where TKey : notnull {
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
