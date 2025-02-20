namespace DotNetToolbox.Results;

public sealed record ValidationResult
    : Result<ValidationState>
    , IValidationResult
    , IResultOperators<ValidationResult> {
    [SetsRequiredMembers]
    internal ValidationResult(IEnumerable<ResultError>? errors = null)
        : base(errors?.Any() ?? false ? ValidationState.Failure : ValidationState.Success, errors ?? []) {
        State = HasErrors ? ValidationState.Failure : ValidationState.Success;
    }

    public static IValidationResult Default => new ValidationResult();

    public bool IsSuccess => Is(ValidationState.Success);
    public bool IsFailure => Is(ValidationState.Failure);

    public IValidationResult<TOutput> SetOutput<TOutput>(TOutput output) => new ValidationResult<TOutput>(output, Errors);
    public IValidationResult Add(IValidationResult other) {
        foreach (var error in other.Errors) Errors.Add(error);
        return this;
    }

    public static IValidationResult From(params IEnumerable<ResultError> errors) => new ValidationResult(errors);
    public static IValidationResult<TOutput> From<TOutput>(TOutput output, params IEnumerable<ResultError> errors) => new ValidationResult<TOutput>(output, errors);
    public static IValidationResult Success() => new ValidationResult();
    public static IValidationResult<TOutput> Success<TOutput>(TOutput output) => new ValidationResult<TOutput>(output);
    public static IValidationResult Failure(params IEnumerable<ResultError> errors) => new ValidationResult(IsNotEmpty(errors));
    public static IValidationResult Failure(string message, string? source = null) => Failure(new ResultError(message, source ?? string.Empty));

    public static implicit operator ValidationResult(string error) => (ResultErrors)error;
    public static implicit operator ValidationResult(ResultError error) => (ResultErrors)error;
    public static implicit operator ValidationResult(ResultError[] errors) => (ResultErrors)errors;
    public static implicit operator ValidationResult(List<ResultError> errors) => (ResultErrors)errors;
    public static implicit operator ValidationResult(HashSet<ResultError> errors) => (ResultErrors)errors;
    public static implicit operator ValidationResult(ResultErrors errors) => new(errors.AsEnumerable());
    public static implicit operator ResultError[](ValidationResult result) => [..result.Errors];
    public static implicit operator List<ResultError>(ValidationResult result) => [.. result.Errors];
    public static implicit operator HashSet<ResultError>(ValidationResult result) => [.. result.Errors];
    public static implicit operator ResultErrors(ValidationResult result) => [.. result.Errors];
    public static implicit operator ValidationState(ValidationResult result) => result.State;

    public static ValidationResult operator +(ValidationResult left, IValidationResult? right)
        => right is null ? left : new(left.Errors.Union([.. right.Errors]));
    public static ValidationResult operator +(ValidationResult left, IEnumerable<ResultError>? right)
        => right is null ? left : new(left.Errors.Union(right));
    public static ValidationResult operator +(ValidationResult left, ResultError? right)
        => right is null ? left : new(left.Errors.Union([right]));
    public static ValidationResult operator +(ValidationResult left, string? right)
        => right is null ? left : new(left.Errors.Union([right]));

    #pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    public bool Equals(ValidationResult? other)
        => other is not null && Errors.SequenceEqual(other.Errors);
    #pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.
}

public sealed record ValidationResult<TOutput>
    : Result<ValidationState, TOutput>
    , IValidationResult<TOutput>
    , IResultOperators<ValidationResult<TOutput>, TOutput> {
    [SetsRequiredMembers]
    internal ValidationResult(TOutput output, IEnumerable<ResultError>? errors = null)
        : base((errors ?? []).Any() ? ValidationState.Failure : ValidationState.Success, output, errors) {
    }

    [SetsRequiredMembers]
    private ValidationResult(IEnumerable<ResultError>? errors = null)
        : this(default!, errors) {
    }

    public bool IsSuccess => Is(ValidationState.Success);
    public bool IsFailure => Is(ValidationState.Failure);

    public IValidationResult<T> SetOutput<T>(T output) => new ValidationResult<T>(output, Errors);
    IValidationResult IValidationResult.Add(IValidationResult other) => MergeWith(other);
    public IValidationResult<TOutput> MergeWith(IValidationResult other) {
        foreach (var error in other.Errors) Errors.Add(error);
        return this;
    }

    public static implicit operator ValidationResult<TOutput>(TOutput output) => new(output);
    public static implicit operator ValidationResult<TOutput>(string error) => (ResultErrors)error;
    public static implicit operator ValidationResult<TOutput>(ResultError error) => (ResultErrors)error;
    public static implicit operator ValidationResult<TOutput>(ResultError[] error) => (ResultErrors)error;
    public static implicit operator ValidationResult<TOutput>(List<ResultError> error) => (ResultErrors)error;
    public static implicit operator ValidationResult<TOutput>(HashSet<ResultError> error) => (ResultErrors)error;
    public static implicit operator ValidationResult<TOutput>(ResultErrors errors) => new(errors.AsEnumerable());
    public static implicit operator ValidationResult<TOutput>(ValidationResult result) => new(result.Errors.AsEnumerable());
    public static implicit operator ResultError[](ValidationResult<TOutput> result) => [.. result.Errors];
    public static implicit operator List<ResultError>(ValidationResult<TOutput> result) => [.. result.Errors];
    public static implicit operator HashSet<ResultError>(ValidationResult<TOutput> result) => [.. result.Errors];
    public static implicit operator ResultErrors(ValidationResult<TOutput> result) => [.. result.Errors];
    public static implicit operator ValidationResult(ValidationResult<TOutput> result) => new(result.Errors);
    public static implicit operator TOutput(ValidationResult<TOutput> result) => result.Output;
    public static implicit operator ValidationState(ValidationResult<TOutput> result) => result.State;

    public static ValidationResult<TOutput> operator +(ValidationResult<TOutput> left, IValidationResult? right)
        => right is null ? left : new(left.Output, left.Errors.Union(right.Errors));
    public static ValidationResult<TOutput> operator +(ValidationResult<TOutput> left, IEnumerable<ResultError>? right)
        => right is null ? left : new(left.Output, left.Errors.Union(right));
    public static ValidationResult<TOutput> operator +(ValidationResult<TOutput> left, ResultError? right)
        => right is null ? left : new(left.Output, left.Errors.Union([right]));
    public static ValidationResult<TOutput> operator +(ValidationResult<TOutput> left, string? right)
        => right is null ? left : new(left.Output, left.Errors.Union([right]));

    #pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    public bool Equals(ValidationResult<TOutput>? other)
        => other is not null && State == other.State && (Output?.Equals(other.Output) ?? other.Output is null) && Errors.SequenceEqual(other.Errors);
    #pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.
}
