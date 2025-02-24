namespace DotNetToolbox.Results;

public enum SignInStatus {
    Pending,
    InvalidInput,
    UserNotFound,
    Incorrect,
    BlockedUser,
    LockedUser,
    NotConfirmed,
    RequiresTwoFactor,
    Success,
}
