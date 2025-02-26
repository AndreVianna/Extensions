// ReSharper disable once CheckNamespace - Intended to be in this namespace
namespace System.Collections;

public record IndexedItem<TValue> : Indexed<TValue> {
    public IndexedItem(int index, TValue value, bool isLast)
        : base(index, value) {
        IsLast = isLast;
    }

    public bool IsFirst => Index == 0;
    public bool IsLast { get; init; }

    public void Deconstruct(out int index, out TValue value, out bool isLast) {
        index = Index;
        value = Value;
        isLast = IsLast;
    }
}
