namespace DotNetToolbox.Results;

public sealed record SignInResult
    : Result<SignInState>
    , ISignInResult
    , IResultOperators<SignInResult> {
    [SetsRequiredMembers]
    private SignInResult(SignInState state, IEnumerable<ResultError>? errors = null)
        : base(state, errors ?? []) {
        if (HasErrors) State = SignInState.Invalid;
    }

    public static ISignInResult Default => new SignInResult(SignInState.NotAttempted);

    public bool IsInvalid => Is(SignInState.Invalid);
    public bool IsLocked => Is(SignInState.Locked);
    public bool IsBlocked => Is(SignInState.Blocked);
    public bool IsIncorrect => Is(SignInState.Incorrect);
    public bool IsNotFound => Is(SignInState.NotFound);
    public bool IsConfirmationPending => Is(SignInState.ConfirmationPending);
    public bool IsTwoFactorRequired => Is(SignInState.TwoFactorRequired);
    public bool IsSuccess => Is(SignInState.Success);

    public ISignInResult MergeWith(IValidationResult other) {
        foreach (var error in other.Errors) Errors.Add(error);
        return this;
    }

    public static ISignInResult Invalid(params IEnumerable<ResultError> errors) => new SignInResult(SignInState.Invalid, errors: errors);
    public static ISignInResult Invalid(string message, string? source = null) => new SignInResult(SignInState.Invalid, [new(message, source ?? string.Empty)]);
    public static ISignInResult NotFound() => new SignInResult(SignInState.NotFound);
    public static ISignInResult Blocked() => new SignInResult(SignInState.Blocked);
    public static ISignInResult Locked() => new SignInResult(SignInState.Locked);
    public static ISignInResult Incorrect() => new SignInResult(SignInState.Incorrect);
    public static ISignInResult ConfirmationPending() => new SignInResult(SignInState.ConfirmationPending);
    public static ISignInResult TwoFactorRequired() => new SignInResult(SignInState.TwoFactorRequired);
    public static ISignInResult Success() => new SignInResult(SignInState.Success);

    public static implicit operator SignInResult(string error) => (ValidationResult)error;
    public static implicit operator SignInResult(ResultError error) => (ValidationResult)error;
    public static implicit operator SignInResult(ResultError[] errors) => (ValidationResult)errors;
    public static implicit operator SignInResult(List<ResultError> errors) => (ValidationResult)errors;
    public static implicit operator SignInResult(HashSet<ResultError> errors) => (ValidationResult)errors;
    public static implicit operator SignInResult(ResultErrors errors) => (ValidationResult)errors;
    public static implicit operator SignInResult(ValidationResult result) => new(SignInState.NotAttempted, errors: result.Errors);
    public static implicit operator ResultError[](SignInResult result) => [.. result.Errors];
    public static implicit operator List<ResultError>(SignInResult result) => [.. result.Errors];
    public static implicit operator HashSet<ResultError>(SignInResult result) => [.. result.Errors];
    public static implicit operator ResultErrors(SignInResult result) => [.. result.Errors];
    public static implicit operator ValidationResult(SignInResult result) => new(result.Errors);
    public static implicit operator SignInState(SignInResult result) => result.State;

    public static SignInResult operator +(SignInResult left, IValidationResult? right)
        => right is null ? left : new(left.State, left.Errors.Union([.. right.Errors]));
    public static SignInResult operator +(SignInResult left, IEnumerable<ResultError>? right)
        => right is null ? left : new(left.State, [.. left.Errors.Union(right)]);
    public static SignInResult operator +(SignInResult left, ResultError? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));
    public static SignInResult operator +(SignInResult left, string? right)
        => right is null ? left : new(left.State, left.Errors.Union([right]));

    #pragma warning disable CS8851 // Record defines 'Equals' but not 'GetHashCode'.
    public bool Equals(SignInResult? other)
        => other is not null && State == other.State && Errors.SequenceEqual(other.Errors);
    #pragma warning restore CS8851 // Record defines 'Equals' but not 'GetHashCode'.
}
