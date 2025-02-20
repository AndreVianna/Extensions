namespace DotNetToolbox.Results;

public enum SignInState : byte {
    NotAttempted = 0, // No attempt was made.
    Invalid = 1, // request validation failed. No attempt was made.
    NotFound = 2, // account not found. Counts as Incorrect.
    Blocked = 3, // account is blocked. Counts as Incorrect.
    Locked = 4, // account is locked. Counts as Incorrect.
    Incorrect = 5, // attempt failed.
    ConfirmationPending = 6, // attempt succeeded but email is not confirmed.
    TwoFactorRequired = 7, // attempt succeeded, but requires 2-factor authentication.
    Success = 8, // attempt succeeded.
}
