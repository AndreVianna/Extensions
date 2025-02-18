using static DotNetToolbox.Results.SignInState;

namespace DotNetToolbox.Results;

public record SignInResult
    : OperationResult<SignInState>
    , ISignInResult
    , IResultOperations<SignInResult> {
    [SetsRequiredMembers]
    internal SignInResult(SignInState state = Pending, string? token = null, IEnumerable<OperationError>? errors = null)
        : base(state, errors ?? []) {
        if (HasErrors) State = Invalid;
        Token = string.IsNullOrWhiteSpace(token) ? null : token.Trim();
    }

    public bool IsPending => Is(Pending);
    public bool IsInvalid => Is(Invalid);
    public bool IsNotFound => Is(NotFound);
    public bool IsLocked => Is(Locked);
    public bool IsBlocked => Is(Blocked);
    public bool IsIncorrect => Is(Incorrect);
    public bool IsConfirmationPending => Is(ConfirmationPending);
    public bool IsTwoFactorRequired => Is(TwoFactorRequired);
    public bool IsSuccess => Is(Success);

    public string? Token { get; }

    public static SignInResult AdditiveIdentity => new();

    public static implicit operator SignInResult(string error) => (Result)error;
    public static implicit operator SignInResult(OperationError error) => (Result)error;
    public static implicit operator SignInResult(OperationError[] errors) => (Result)errors;
    public static implicit operator SignInResult(List<OperationError> errors) => (Result)errors;
    public static implicit operator SignInResult(HashSet<OperationError> errors) => (Result)errors;
    public static implicit operator SignInResult(OperationErrors errors) => (Result)errors;
    public static implicit operator SignInResult(Result result) => new(Success, errors: result.Errors.AsEnumerable());
    public static implicit operator OperationError[](SignInResult result) => [.. result.Errors];
    public static implicit operator List<OperationError>(SignInResult result) => [.. result.Errors];
    public static implicit operator HashSet<OperationError>(SignInResult result) => [.. result.Errors];
    public static implicit operator OperationErrors(SignInResult result) => [.. result.Errors];
    public static implicit operator Result(SignInResult result) => new(result.Errors);
    public static implicit operator SignInState(SignInResult result) => result.State;

    public static SignInResult operator +(SignInResult left, IResult? right)
        => right is null ? left : new(left.State, left.Token, left.Errors.Union([.. right.Errors]));
    public static SignInResult operator +(SignInResult left, IEnumerable<OperationError>? right)
        => right is null ? left : new(left.State, left.Token, [.. left.Errors.Union(right)]);
    public static SignInResult operator +(SignInResult left, OperationError? right)
        => right is null ? left : new(left.State, left.Token, left.Errors.Union([right]));
    public static SignInResult operator +(SignInResult left, string? right)
        => right is null ? left : new(left.State, left.Token, left.Errors.Union([right]));
}
