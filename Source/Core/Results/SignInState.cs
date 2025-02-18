namespace DotNetToolbox.Results;

public enum SignInState : byte {
    Pending = 0, // attempt succeeded.
    Invalid = 1, // request validation failed. No attempt was made.
    Blocked = 2, // account is blocked. Counts as Incorrect.
    Locked = 3, // account is locked. Counts as Incorrect.
    Incorrect = 4, // attempt failed.
    NotFound = 5, // attempt failed.
    ConfirmationPending = 6, // attempt succeeded but email is not confirmed.
    TwoFactorRequired = 7, // attempt succeeded, but requires 2-factor authentication.
    Success = 8, // attempt succeeded.
}
