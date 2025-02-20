namespace DotNetToolbox.Sequencers;

public interface ISequencer<TValue>
    : IEnumerator<TValue>
    where TValue : notnull {
    TValue First { get; }
    new TValue Current { get; set; }

    void Set(TValue value, bool skip = false);
}
