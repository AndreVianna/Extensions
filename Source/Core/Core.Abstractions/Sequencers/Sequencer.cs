namespace DotNetToolbox.Sequencers;

public abstract class Sequencer<TSequencer, TValue>
    : ISequencer<TValue>
    where TSequencer : Sequencer<TSequencer, TValue>
    where TValue : notnull {
    private bool _disposed;
    private TValue _current;

    protected Sequencer(TValue start) {
        First = start;
        Reset();
    }

    public TValue First { get; }
    object IEnumerator.Current => _current;
    public TValue Current {
        get {
            MoveNext();
            return _current;
        }
        set => Set(value);
    }

    [MemberNotNull(nameof(_current))]
    public void Reset()
        => _current = First;

    [MemberNotNull(nameof(_current))]
    public void Set(TValue value, bool skip = false) {
        _current = value;
        if (skip) MoveNext();
    }

    [MemberNotNullWhen(true, nameof(_current))]
    public bool MoveNext() {
        var hasNext = TryGenerateNext(_current, out var next);
        if (hasNext) _current = next;
        return hasNext;
    }

    [MemberNotNullWhen(true, nameof(_current))]
    protected abstract bool TryGenerateNext(TValue? current, out TValue next);

    protected virtual void Dispose(bool disposing) {
        if (_disposed) return;
        if (disposing) {
            if (First is IDisposable disposableFirst) disposableFirst.Dispose();
            if (_current is IDisposable disposableCurrent) disposableCurrent.Dispose();
        }

        _disposed = true;
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
