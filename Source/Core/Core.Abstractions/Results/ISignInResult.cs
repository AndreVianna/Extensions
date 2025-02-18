namespace DotNetToolbox.Results;

public interface ISignInResult
    : IHasErrors {
    bool IsPending { get; } // No attempt was made.
    bool IsInvalid { get; } // request validation failed. No attempt was made.
    bool IsBlocked { get; } // account is blocked. Counts as Failed.
    bool IsLocked { get; } // account is locked. Counts as Failed.
    bool IsIncorrect { get; } // incorrect sign in secret provided.
    bool IsNotFound { get; } // user not found.
    bool IsConfirmationPending { get; } // attempt succeeded but email is not confirmed.
    bool IsTwoFactorRequired { get; } // attempt succeeded, but requires 2-factor authentication.
    bool IsSuccess { get; } // attempt succeeded.

    string? Token { get; }
}
