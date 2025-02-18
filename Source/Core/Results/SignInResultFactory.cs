namespace DotNetToolbox.Results;

public static class SignInResultFactory {
    private static ISignInResult Create(SignInState type, string? token = null, params IEnumerable<OperationError> errors)
        => new SignInResult(type, token, errors);

    public static ISignInResult Success(string? token = null)
        => Create(SignInState.Success, token);
    public static ISignInResult ConfirmationPending(string token)
        => Create(SignInState.ConfirmationPending, token);
    public static ISignInResult TwoFactorRequired(string token)
        => Create(SignInState.ConfirmationPending, token);
    public static ISignInResult Invalid(params IEnumerable<OperationError> errors)
        => Create(SignInState.Invalid, errors: errors);
    public static ISignInResult Invalid(string message, string? source = null)
        => Invalid(new OperationError(message, source ?? string.Empty));
    public static ISignInResult Blocked()
        => Create(SignInState.Blocked);
    public static ISignInResult Locked()
        => Create(SignInState.Locked);
    public static ISignInResult Failed(params IEnumerable<OperationError> errors)
        => Create(SignInState.Incorrect, errors: errors);
    public static ISignInResult Failed(string message, string? source = null)
        => Create(SignInState.Incorrect, errors: new OperationError(message, source ?? string.Empty));

    public static Task<ISignInResult> SuccessAsync(string? token = null)
        => Task.FromResult(Success(token));
    public static Task<ISignInResult> ConfirmationPendingAsync(string token)
        => Task.FromResult(ConfirmationPending(token));
    public static Task<ISignInResult> TwoFactorRequiredAsync(string token)
        => Task.FromResult(TwoFactorRequired(token));
    public static Task<ISignInResult> InvalidAsync(params IEnumerable<OperationError> errors)
        => Task.FromResult(Invalid(errors));
    public static Task<ISignInResult> InvalidAsync(string message, string? source = null)
        => Task.FromResult(Invalid(message, source));
    public static Task<ISignInResult> BlockedAsync()
        => Task.FromResult(Blocked());
    public static Task<ISignInResult> LockedAsync()
        => Task.FromResult(Locked());
    public static Task<ISignInResult> FailedAsync(params IEnumerable<OperationError> errors)
        => Task.FromResult(Failed(errors));
    public static Task<ISignInResult> FailedAsync(string message, string? source = null)
        => Task.FromResult(Failed(message, source));
}
