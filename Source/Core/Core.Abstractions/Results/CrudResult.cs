namespace DotNetToolbox.Results;

public sealed record CrudResult
    : Result<CrudState>
    , ICrudResult
    , IResultOperators<CrudResult> {
    [SetsRequiredMembers]
    private CrudResult(CrudState state, IEnumerable<ResultError>? errors = null)
        : base(state, errors ?? []) {
        if (HasErrors) State = CrudState.Invalid;
    }

    [SetsRequiredMembers]
    private CrudResult(IEnumerable<ResultError>? errors = null)
        : this(default, errors) {
    }

    public static ICrudResult Default => new CrudResult();

    public bool IsSuccess => Is(CrudState.Success);
    public bool IsInvalid => Is(CrudState.Invalid);
    public bool IsNotFound => Is(CrudState.NotFound);
    public bool IsConflict => Is(CrudState.Conflict);

    public ICrudResult<TOutput> SetOutput<TOutput>(TOutput output) => new CrudResult<TOutput>(State, output, Errors);
    public ICrudResult MergeWith(IValidationResult other) {
        foreach (var error in other.Errors) Errors.Add(error);
        return this;
    }

    public static ICrudResult Success() => new CrudResult();
    public static ICrudResult<TOutput> Success<TOutput>(TOutput output)
        => new CrudResult<TOutput>(output);
    public static ICrudResult Invalid(params IEnumerable<ResultError> errors)
        => new CrudResult(CrudState.Invalid, errors);
    public static ICrudResult Invalid(string message, string? source = null)
        => Invalid(new ResultError(message, source ?? string.Empty));
    public static ICrudResult NotFound(params IEnumerable<ResultError> errors)
        => new CrudResult(CrudState.NotFound, errors);
    public static ICrudResult NotFound(string message, string? source = null)
        => Conflict(new ResultError(message, source ?? string.Empty));
    public static ICrudResult Conflict(params IEnumerable<ResultError> errors)
        => new CrudResult(CrudState.Conflict, errors);
    public static ICrudResult Conflict(string message, string? source = null)
        => Conflict(new ResultError(message, source ?? string.Empty));

    public static implicit operator CrudResult(string error) => (ValidationResult)error;
    public static implicit operator CrudResult(ResultError error) => (ValidationResult)error;
    public static implicit operator CrudResult(ResultError[] errors) => (ValidationResult)errors;
    public static implicit operator CrudResult(List<ResultError> errors) => (ValidationResult)errors;
    public static implicit operator CrudResult(HashSet<ResultError> errors) => (ValidationResult)errors;
    public static implicit operator CrudResult(ResultErrors errors) => (ValidationResult)errors;
    public static implicit operator CrudResult(ValidationResult result) => new(result.Errors.AsEnumerable());
    public static implicit operator ResultError[](CrudResult result) => [.. result.Errors];
    public static implicit operator List<ResultError>(CrudResult result) => [.. result.Errors];
    public static implicit operator HashSet<ResultError>(CrudResult result) => [.. result.Errors];
    public static implicit operator ResultErrors(CrudResult result) => [.. result.Errors];
    public static implicit operator ValidationResult(CrudResult result) => new(result.Errors);
    public static implicit operator CrudState(CrudResult result) => result.State;

    public static CrudResult operator +(CrudResult left, IValidationResult? right)
        => right is null ? left : new(left.State, left.Errors.Union([.. right.Errors]));
    public static CrudResult operator +(CrudResult left, IEnumerable<ResultError>? right)
        => right is null ? left : new(left.State, [.. left.Errors.Union(right)]);
    public static CrudResult operator +(CrudResult left, ResultError? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));
    public static CrudResult operator +(CrudResult left, string? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));

    #pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    public bool Equals(CrudResult? other)
        => other is not null && State == other.State && Errors.SequenceEqual(other.Errors);
    #pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.
}

public sealed record CrudResult<TOutput>
    : Result<CrudState, TOutput>
    , ICrudResult<TOutput>
    , IResultOperators<CrudResult<TOutput>, TOutput> {
    [SetsRequiredMembers]
    internal CrudResult(CrudState state, TOutput output = default!, IEnumerable<ResultError>? errors = null)
        : base(state, output, errors ?? []) {
        if (HasErrors) State = CrudState.Invalid;
    }
    [SetsRequiredMembers]
    internal CrudResult(TOutput output = default!, IEnumerable<ResultError>? errors = null)
        : this(default, output, errors ?? []) {
    }

    ICrudResult<T> ICrudResult.SetOutput<T>(T output) => throw new NotSupportedException();
    ICrudResult ICrudResult.MergeWith(IValidationResult other) => MergeWith(other);
    public ICrudResult<TOutput> MergeWith(IValidationResult other) {
        foreach (var error in other.Errors) Errors.Add(error);
        return this;
    }

    public bool IsSuccess => Is(CrudState.Success);
    public bool IsInvalid => Is(CrudState.Invalid);
    public bool IsNotFound => Is(CrudState.NotFound);
    public bool IsConflict => Is(CrudState.Conflict);

    public static implicit operator CrudResult<TOutput>(TOutput output) => new(output);
    public static implicit operator CrudResult<TOutput>(string error) => (ValidationResult<TOutput>)error;
    public static implicit operator CrudResult<TOutput>(ResultError error) => (ValidationResult<TOutput>)error;
    public static implicit operator CrudResult<TOutput>(ResultErrors errors) => (ValidationResult<TOutput>)errors;
    public static implicit operator CrudResult<TOutput>(ResultError[] errors) => (ValidationResult<TOutput>)errors;
    public static implicit operator CrudResult<TOutput>(List<ResultError> errors) => (ValidationResult<TOutput>)errors;
    public static implicit operator CrudResult<TOutput>(HashSet<ResultError> errors) => (ValidationResult<TOutput>)errors;
    public static implicit operator CrudResult<TOutput>(ValidationResult<TOutput> result) => new(result.Output, result.Errors);
    public static implicit operator ResultError[](CrudResult<TOutput> result) => [.. result.Errors];
    public static implicit operator List<ResultError>(CrudResult<TOutput> result) => [.. result.Errors];
    public static implicit operator HashSet<ResultError>(CrudResult<TOutput> result) => [.. result.Errors];
    public static implicit operator ResultErrors(CrudResult<TOutput> result) => [.. result.Errors];
    public static implicit operator ValidationResult<TOutput>(CrudResult<TOutput> result) => new(result.Output, result.Errors);
    public static implicit operator CrudState(CrudResult<TOutput> result) => result.State;
    public static implicit operator TOutput(CrudResult<TOutput> result) => result.Output;

    public static CrudResult<TOutput> operator +(CrudResult<TOutput> left, IValidationResult? right)
        => right is null ? left : new(left.State, left.Output, left.Errors.Union([.. right.Errors]));
    public static CrudResult<TOutput> operator +(CrudResult<TOutput> left, IEnumerable<ResultError>? right)
        => right is null ? left : new(left.State, left.Output, [.. left.Errors.Union(right)]);
    public static CrudResult<TOutput> operator +(CrudResult<TOutput> left, ResultError? right)
        => right is null ? left : new(left.State, left.Output, left.Errors.Union([right]));
    public static CrudResult<TOutput> operator +(CrudResult<TOutput> left, string? right)
        => right is null ? left : new(left.State, left.Output, left.Errors.Union([right]));

    #pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    public bool Equals(CrudResult<TOutput>? other)
        => other is not null && State == other.State && (Output?.Equals(other.Output) ?? other.Output is null) && Errors.SequenceEqual(other.Errors);
    #pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.
}
