namespace DotNetToolbox;

[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
[SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>")]
public class OperationErrors : ISet<OperationError> {
    private readonly HashSet<OperationError> _errors = [];
    public OperationErrors(params IEnumerable<OperationError> errors) {
        _errors.UnionWith(errors);
    }

    public static implicit operator OperationErrors(string? error) => error is null ? [] : (OperationError)error;
    public static implicit operator OperationErrors(OperationError? error) => error is null ? [] : new(error);
    public static implicit operator OperationErrors(string[]? errors) => errors is null ? [] : [.. errors];
    public static implicit operator OperationErrors(OperationError[]? errors) => errors is null ? [] : [.. errors];
    public static implicit operator OperationErrors(List<string>? errors) => errors is null ? [] : [.. errors];
    public static implicit operator OperationErrors(List<OperationError>? errors) => errors is null ? [] : [.. errors];
    public static implicit operator OperationErrors(HashSet<string>? errors) => errors is null ? [] : [.. errors];
    public static implicit operator OperationErrors(HashSet<OperationError>? errors) => errors is null ? [] : [.. errors];

    public static implicit operator OperationError[](OperationErrors? errors) => errors is null ? [] : [.. errors];
    public static implicit operator List<OperationError>(OperationErrors? errors) => errors is null ? [] : [.. errors];
    public static implicit operator HashSet<OperationError>(OperationErrors? errors) => errors is null ? [] : [.. errors];

    public static OperationErrors operator +(OperationErrors left, OperationErrors? right)
        => right is null ? left : [.. left.Union(right)];
    public static OperationErrors operator +(OperationErrors left, OperationError? right)
        => right is null ? left : [.. left.Union([right])];
    public static OperationErrors operator +(OperationErrors left, string? right)
        => right is null ? left : [.. left.Union([right])];

    public IEnumerator<OperationError> GetEnumerator() => _errors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_errors).GetEnumerator();
    public void ExceptWith(IEnumerable<OperationError> other) => _errors.ExceptWith(other);
    public void IntersectWith(IEnumerable<OperationError> other) => _errors.IntersectWith(other);
    public bool IsProperSubsetOf(IEnumerable<OperationError> other) => _errors.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<OperationError> other) => _errors.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<OperationError> other) => _errors.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<OperationError> other) => _errors.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<OperationError> other) => _errors.Overlaps(other);
    public bool SetEquals(IEnumerable<OperationError> other) => _errors.SetEquals(other);
    public void SymmetricExceptWith(IEnumerable<OperationError> other) => _errors.SymmetricExceptWith(other);
    public void UnionWith(IEnumerable<OperationError> other) => _errors.UnionWith(other);

    bool ISet<OperationError>.Add(OperationError item) => _errors.Add(item);
    void ICollection<OperationError>.Add(OperationError item) => _errors.Add(item);
    public bool Add(OperationError value) => _errors.Add(value);

    public void Clear() => _errors.Clear();
    public bool Contains(OperationError item) => _errors.Contains(item);
    public void CopyTo(OperationError[] array, int arrayIndex) => _errors.CopyTo(array, arrayIndex);
    public bool Remove(OperationError item) => _errors.Remove(item);
    public int Count => _errors.Count;
    public bool IsReadOnly => false;
}
