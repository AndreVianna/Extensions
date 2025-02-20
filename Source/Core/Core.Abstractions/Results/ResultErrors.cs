namespace DotNetToolbox.Results;

[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "<Pending>")]
[SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>")]
public class ResultErrors : ISet<ResultError> {
    private readonly HashSet<ResultError> _errors = [];
    public ResultErrors(params IEnumerable<ResultError> errors) {
        _errors.UnionWith(errors);
    }

    public static implicit operator ResultErrors(string? error) => error is null ? [] : (ResultError)error;
    public static implicit operator ResultErrors(ResultError? error) => error is null ? [] : new(error);
    public static implicit operator ResultErrors(string[]? errors) => errors is null ? [] : [.. errors];
    public static implicit operator ResultErrors(ResultError[]? errors) => errors is null ? [] : [.. errors];
    public static implicit operator ResultErrors(List<string>? errors) => errors is null ? [] : [.. errors];
    public static implicit operator ResultErrors(List<ResultError>? errors) => errors is null ? [] : [.. errors];
    public static implicit operator ResultErrors(HashSet<string>? errors) => errors is null ? [] : [.. errors];
    public static implicit operator ResultErrors(HashSet<ResultError>? errors) => errors is null ? [] : [.. errors];

    public static implicit operator ResultError[](ResultErrors? errors) => errors is null ? [] : [.. errors];
    public static implicit operator List<ResultError>(ResultErrors? errors) => errors is null ? [] : [.. errors];
    public static implicit operator HashSet<ResultError>(ResultErrors? errors) => errors is null ? [] : [.. errors];

    public static ResultErrors operator +(ResultErrors left, ResultErrors? right)
        => right is null ? left : [.. left.Union(right)];
    public static ResultErrors operator +(ResultErrors left, ResultError? right)
        => right is null ? left : [.. left.Union([right])];
    public static ResultErrors operator +(ResultErrors left, string? right)
        => right is null ? left : [.. left.Union([right])];

    public IEnumerator<ResultError> GetEnumerator() => _errors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_errors).GetEnumerator();
    public void ExceptWith(IEnumerable<ResultError> other) => _errors.ExceptWith(other);
    public void IntersectWith(IEnumerable<ResultError> other) => _errors.IntersectWith(other);
    public bool IsProperSubsetOf(IEnumerable<ResultError> other) => _errors.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<ResultError> other) => _errors.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<ResultError> other) => _errors.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<ResultError> other) => _errors.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<ResultError> other) => _errors.Overlaps(other);
    public bool SetEquals(IEnumerable<ResultError> other) => _errors.SetEquals(other);
    public void SymmetricExceptWith(IEnumerable<ResultError> other) => _errors.SymmetricExceptWith(other);
    public void UnionWith(IEnumerable<ResultError> other) => _errors.UnionWith(other);

    bool ISet<ResultError>.Add(ResultError item) => _errors.Add(item);
    void ICollection<ResultError>.Add(ResultError item) => _errors.Add(item);
    public bool Add(ResultError value) => _errors.Add(value);

    public void Clear() => _errors.Clear();
    public bool Contains(ResultError item) => _errors.Contains(item);
    public void CopyTo(ResultError[] array, int arrayIndex) => _errors.CopyTo(array, arrayIndex);
    public bool Remove(ResultError item) => _errors.Remove(item);
    public int Count => _errors.Count;
    public bool IsReadOnly => false;
}
