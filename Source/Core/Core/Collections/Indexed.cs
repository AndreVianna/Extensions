namespace System.Collections;

public record Indexed<TValue> {
    public Indexed(int index, TValue value) {
        Index = index < 0 ? 0 : index;
        Value = value;
    }

    public int Index { get; init; }
    public TValue Value { get; init; }

    public void Deconstruct(out int index, out TValue value) {
        index = Index;
        value = Value;
    }
}
