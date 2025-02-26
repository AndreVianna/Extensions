namespace DotNetToolbox.Sequencers;

public abstract class KeySequencer<TKey>(TKey start)
    : Sequencer<KeySequencer<TKey>, TKey>(start)
    where TKey : IEquatable<TKey>, IComparable<TKey>;
