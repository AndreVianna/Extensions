using static DotNetToolbox.Results.SignInResult;

namespace DotNetToolbox.Results;

public class SignInResultTests {
    private static readonly SignInResult _invalid = new ValidationError("Some error.", "Source");
    private static readonly SignInResult _invalidWithSameError = new ValidationError("Some error.", "Source");
    private static readonly SignInResult _invalidWithOtherError = new ValidationError("Other error.", "Source");
    private static readonly SignInResult _locked = LockedAccount();
    private static readonly SignInResult _blocked = BlockedAccount();
    private static readonly SignInResult _failure = FailedAttempt();
    private static readonly SignInResult _requiresConfirmation = ConfirmationIsPending("SomeToken");
    private static readonly SignInResult _requires2Factor = TwoFactorIsRequired("SomeToken");
    private static readonly SignInResult _success = Success("SomeToken");
    private static readonly SignInResult _successWithSameToken = Success("SomeToken");
    private static readonly SignInResult _successWithOtherToken = Success("OtherToken");

    [Fact]
    public void Success_CreatesResult() {
        // Arrange & Act
        var result = Success("SomeToken") with { };

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Exception.Should().BeNull();
        result.Token.Should().Be("SomeToken");
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void RequiresTwoFactor_CreatesResult() {
        // Arrange & Act
        var result = TwoFactorIsRequired("SomeToken");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Exception.Should().BeNull();
        result.Token.Should().Be("SomeToken");
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeTrue();
        result.IsInvalid.Should().BeFalse();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void RequiresConfirmation_CreatesResult() {
        // Arrange & Act
        var result = ConfirmationIsPending("SomeToken");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Exception.Should().BeNull();
        result.Token.Should().Be("SomeToken");
        result.RequiresConfirmation.Should().BeTrue();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void InvalidRequest_WithMessageOnly_CreatesResult() {
        // Arrange & Act
        var result = InvalidRequest("Some error.");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Exception.Should().BeNull();
        result.Token.Should().BeNull();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void InvalidRequest_WithSourceAndMessage_CreatesResult() {
        // Arrange & Act
        var result = InvalidRequest(new ValidationError("Some error.", "Field1"));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Exception.Should().BeNull();
        result.Token.Should().BeNull();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void InvalidRequest_WithResult_CreatesResult() {
        // Arrange & Act
        var result = InvalidRequest(Result.Invalid("Some error."));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Exception.Should().BeNull();
        result.Token.Should().BeNull();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void BlockedAccount_WithMessageOnly_CreatesResult() {
        // Arrange & Act
        var result = BlockedAccount();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Exception.Should().BeNull();
        result.Token.Should().BeNull();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsBlocked.Should().BeTrue();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void LockedAccount_CreatesResult() {
        // Arrange & Act
        var result = LockedAccount();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Exception.Should().BeNull();
        result.Token.Should().BeNull();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void FailedAttempt_CreatesResult() {
        // Arrange & Act
        var result = FailedAttempt();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Exception.Should().BeNull();
        result.Token.Should().BeNull();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ErrorAttempt_CreatesResult() {
        // Arrange & Act
        var result = Error(new Exception("Message"));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Exception.Should().NotBeNull();
        result.Token.Should().BeNull();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void ErrorAttempt_WithMessage_CreatesResult() {
        // Arrange & Act
        var result = Error("Message");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Exception.Should().NotBeNull();
        result.Token.Should().BeNull();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsBlocked.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
        result.IsFailure.Should().BeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromValidationError_ReturnsFailure() {
        // Act
        var result = (SignInResult)new ValidationError("Some error.", "Source");

        // Assert
        result.IsInvalid.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorArray_ReturnsFailure() {
        // Act
        SignInResult result = new[] { new ValidationError("Some error.", "Source") };

        // Assert
        result.IsInvalid.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorList_ReturnsFailure() {
        // Act
        SignInResult result = new List<ValidationError> { new("Some error.", "Source") };

        // Assert
        result.IsInvalid.Should().BeTrue();
    }

    private static SignInResult? ToResult(string? result)
        => result switch {
            null => null,
            nameof(_success) => _success,
            nameof(_invalid) => _invalid,
            nameof(_locked) => _locked,
            nameof(_blocked) => _blocked,
            nameof(_requiresConfirmation) => _requiresConfirmation,
            nameof(_requires2Factor) => _requires2Factor,
            nameof(_invalidWithSameError) => _invalidWithSameError,
            nameof(_invalidWithOtherError) => _invalidWithOtherError,
            nameof(_failure) => _failure,
            _ => throw new ArgumentException($"Invalid field name: {result}"),
        };

    private sealed class TestDataForProperties : TheoryData<string, bool, bool, bool, bool, bool, bool, bool> {
        public TestDataForProperties() {
            Add(nameof(_invalid), true, false, false, false, false, false, false);
            Add(nameof(_blocked), false, true, false, false, false, false, false);
            Add(nameof(_locked), false, false, true, false, false, false, false);
            Add(nameof(_failure), false, false, false, true, false, false, false);
            Add(nameof(_requiresConfirmation), false, false, false, false, true, false, false);
            Add(nameof(_requires2Factor), false, false, false, false, false, true, false);
            Add(nameof(_success), false, false, false, false, false, false, true);
        }
    }
    [Theory]
    [ClassData(typeof(TestDataForProperties))]
    public void Properties_ShouldReturnAsExpected(string subject, bool isInvalid, bool isBlocked, bool isLocked, bool isFailure, bool confirmationRequired, bool twoFactorRequired, bool isSuccess) {
        // Assert
        ToResult(subject)!.IsInvalid.Should().Be(isInvalid);
        ToResult(subject)!.IsLocked.Should().Be(isLocked);
        ToResult(subject)!.IsBlocked.Should().Be(isBlocked);
        ToResult(subject)!.IsFailure.Should().Be(isFailure);
        ToResult(subject)!.RequiresConfirmation.Should().Be(confirmationRequired);
        ToResult(subject)!.RequiresTwoFactor.Should().Be(twoFactorRequired);
        ToResult(subject)!.IsSuccess.Should().Be(isSuccess);
    }

    [Fact]
    public void GetHashCode_DifferentiatesAsExpected() {
        var expectedResult = new HashSet<SignInResult> {
            _success,
            _successWithOtherToken,
            _requires2Factor,
            _locked,
            _blocked,
            _failure,
            _invalid,
            _invalidWithOtherError,
        };

        // Act
        var result = new HashSet<SignInResult> {
            _success,
            _success,
            _successWithSameToken,
            _successWithOtherToken,
            _requires2Factor,
            _locked,
            _blocked,
            _failure,
            _invalid,
            _invalid,
            _invalidWithSameError,
            _invalidWithOtherError,
        };

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void AddOperator_WithoutError_ReturnsDoesNothing() {
        // Arrange
        var result = Success("SomeToken");

        // Act
        result += Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsInvalid.Should().BeFalse();
        result.Token.Should().Be("SomeToken");
    }

    [Fact]
    public void AddOperator_WithError_ReturnsInvalid() {
        // Arrange
        var result = Success("SomeToken");

        // Act
        result += new ValidationError("Some error.", "Source");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Token.Should().BeNull();
    }

    [Fact]
    public void AddOperator_WithOtherError_ReturnsBothErrors() {
        // Arrange
        var result = InvalidRequest(new ValidationError("Some error.", "Source"));

        // Act
        result += new ValidationError("Source", "Other error.");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void AddOperator_WithSameError_ReturnsOnlyOneError() {
        // Arrange
        var result = InvalidRequest(new ValidationError("Some error.", "Source"));

        // Act
        result += new ValidationError("Some error.", "Source");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task SuccessTask_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = SuccessTask("Test value");

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task TwoFactorRequiredTask_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = TwoFactorIsRequiredTask("Test value");

        // Assert
        var result = await task;
        result.RequiresTwoFactor.Should().BeTrue();
    }

    [Fact]
    public async Task ConfirmationPendingTask_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = ConfirmationIsPendingTask("Test value");

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeFalse();
        result.RequiresConfirmation.Should().BeTrue();
    }

    [Fact]
    public async Task BlockedAccountTask_ReturnsTaskWithNotFoundCrudResult() {
        // Act
        var task = BlockedAccountTask();

        // Assert
        var result = await task;
        result.IsBlocked.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.IsLocked.Should().BeFalse();
    }

    [Fact]
    public async Task LockedAccountTask_ReturnsTaskWithConflictCrudResult() {
        // Act
        var task = LockedAccountTask();

        // Assert
        var result = await task;
        result.IsFailure.Should().BeFalse();
        result.IsLocked.Should().BeTrue();
        result.IsBlocked.Should().BeFalse();
    }

    [Fact]
    public async Task FailedAttemptTask_ReturnsTaskWithConflictCrudResult() {
        // Act
        var task = FailedAttemptTask();

        // Assert
        var result = await task;
        result.IsFailure.Should().BeTrue();
        result.IsLocked.Should().BeFalse();
        result.IsBlocked.Should().BeFalse();
    }

    [Fact]
    public async Task ErrorTask_ReturnsTaskWithConflictCrudResult() {
        // Act
        var task = ErrorTask("Some error.");

        // Assert
        var result = await task;
        result.HasException.Should().BeTrue();
    }

    [Fact]
    public async Task InvalidTask_ReturnsTaskWithInvalidCrudResult() {
        // Act
        var task = InvalidTask("Some error.");

        // Assert
        var result = await task;
        result.IsInvalid.Should().BeTrue();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task SuccessTaskTValue_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = SuccessTask("SomeToken");

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeTrue();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeFalse();
        result.Token.Should().Be("SomeToken");
    }

    [Fact]
    public async Task TwoFactorRequiredTaskTValue_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = TwoFactorIsRequiredTask("SomeToken");

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeFalse();
        result.RequiresConfirmation.Should().BeFalse();
        result.RequiresTwoFactor.Should().BeTrue();
        result.Token.Should().Be("SomeToken");
    }

    [Fact]
    public async Task ConfirmationPendingTaskTValue_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = ConfirmationIsPendingTask("SomeToken");

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeFalse();
        result.RequiresConfirmation.Should().BeTrue();
        result.RequiresTwoFactor.Should().BeFalse();
        result.Token.Should().Be("SomeToken");
    }

    [Fact]
    public void Error_WithString_CreatesResultWithException() {
        // Arrange
        const string errorMessage = "ErrorWriter message";

        // Act
        var result = Error(errorMessage);

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception!.Message.Should().Be(errorMessage);
    }

    [Fact]
    public void Error_WithException_CreatesResultWithException() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        var result = Error(exception);

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorToSignInResultTValue_ReturnsInvalid() {
        // Act
        SignInResult result = new ValidationError("Some error.", nameof(result));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Token.Should().BeNull();
    }

    [Fact]
    public void AdditionOperator_CombiningSignInResultWithError_ReturnsResultWithError() {
        // Arrange
        var result = Success("SomeToken");
        var error = new ValidationError("ErrorWriter message", "Property");

        // Act
        var combinedResult = result + error;

        // Assert
        combinedResult.HasErrors.Should().BeTrue();
        combinedResult.Errors.Should().ContainSingle(e => e.Message == "ErrorWriter message" && e.Source == "Property");
    }

    [Fact]
    public void TokenProperty_IsSetCorrectly() {
        // Arrange
        const string expectedValue = "Test value";

        // Act
        var result = Success(expectedValue);

        // Assert
        result.Token.Should().Be(expectedValue);
    }

    [Fact]
    public void EnsureIsSuccess_WhenHasErrors_ThrowsValidationException() {
        // Arrange
        var result = InvalidRequest(new ValidationError("ErrorWriter message"));

        // Act & Assert
        var action = () => result.EnsureIsSuccess();

        action.Should().Throw<ValidationException>().WithMessage(ValidationException.DefaultMessage);
    }

    [Fact]
    public void EnsureIsSuccess_WhenHasException_ThrowsValidationException() {
        // Arrange
        var exception = new Exception("Original exception");
        var result = Error(exception);

        // Act & Assert
        var action = () => result.EnsureIsSuccess();

        action.Should().Throw<ValidationException>().WithInnerException<Exception>().WithMessage("Original exception");
    }

    [Fact]
    public void TypeProperty_WhenHasException_ReturnsError() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        var result = Error(exception);

        // Assert
        result.Type.Should().Be(SignInResultState.Error);
    }

    [Fact]
    public async Task ErrorTask_WithString_ReturnsTaskWithErrorSignInResult() {
        // Arrange
        const string errorMessage = "ErrorWriter message";

        // Act
        var task = ErrorTask(errorMessage);
        var result = await task;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception!.Message.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ErrorTask_WithException_ReturnsTaskWithErrorSignInResult() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        var task = ErrorTask(exception);
        var result = await task;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromHttpResultToValidationErrorArray_ReturnsErrors() {
        // Arrange
        var errors = new[] { new ValidationError("ErrorWriter message", "Property") };

        // Act
        ValidationErrors resultErrors = InvalidRequest(errors);
        ValidationError[] errorArray = InvalidRequest(errors);
        Exception? resultException = InvalidRequest(errors);

        // Assert
        resultErrors.Should().BeEquivalentTo(errors);
        errorArray.Should().BeEquivalentTo(errors);
        resultException.Should().BeNull();
    }

    [Fact]
    public void ImplicitConversion_FromHttpResultToException_ReturnsException() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        ValidationErrors resultErrors = Error(exception);
        ValidationError[] errorArray = Error(exception);
        Exception? resultException = Error(exception);

        // Assert
        resultErrors.Should().BeEmpty();
        errorArray.Should().BeEmpty();
        resultException.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromExceptionToSignInResult_CreatesResultWithException() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        SignInResult result = exception;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorsToSignInResult_CreatesResultWithErrors() {
        // Arrange
        var errors = new ValidationErrors {
            new ValidationError("ErrorWriter message", "Property"),
        };

        // Act
        SignInResult result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromHashSetValidationErrorToSignInResult_CreatesResultWithErrors() {
        // Arrange
        var errors = new HashSet<ValidationError> {
            new("ErrorWriter message", "Property"),
        };

        // Act
        SignInResult result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void AdditionOperator_LeftHasException_ReturnsLeft() {
        // Arrange
        var left = Error(new Exception("Left exception"));
        var right = Result.Success();

        // Act
        var result = left + right;

        // Assert
        result.Should().BeSameAs(left);
        result.HasException.Should().BeTrue();
        result.Exception!.Message.Should().Be("Left exception");
    }

    [Fact]
    public void AdditionOperator_RightHasException_ReturnsNewSignInResultWithRightException() {
        // Arrange
        var left = Success("SomeToken");
        var rightException = new Exception("Right exception");
        var right = Result.Error(rightException);

        // Act
        var result = left + right;

        // Assert
        result.Should().NotBeSameAs(left);
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(rightException);
    }

    [Fact]
    public void AdditionOperator_NeitherHasException_ReturnsCombinedErrors() {
        // Arrange
        var leftErrors = new[] { new ValidationError("Left error", "LeftProperty") };
        var rightErrors = new[] { new ValidationError("Right error", "RightProperty") };
        var left = InvalidRequest(leftErrors);
        var right = Result.Invalid(rightErrors);

        // Act
        var result = left + right;

        // Assert
        result.HasException.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(leftErrors.Union(rightErrors));
    }

    [Fact]
    public void AdditionOperator_NeitherHasExceptionAndDuplicateErrors_ReturnsDistinctErrors() {
        // Arrange
        var sharedError = new ValidationError("Shared error", "SharedProperty");
        var leftErrors = new[] { sharedError };
        var rightErrors = new[] { sharedError };
        var left = InvalidRequest(leftErrors);
        var right = Result.Invalid(rightErrors);

        // Act
        var result = left + right;

        // Assert
        result.HasException.Should().BeFalse();
        result.Errors.Should().ContainSingle(); // Only one instance of the shared error
        result.Errors.Should().Contain(sharedError);
    }

    [Fact]
    public void AdditionOperator_CombiningSignInResultTValueWithResultWhenRightHasException_ReturnsNewSignInResultTValueWithRightException() {
        // Arrange
        const string value = "Test value";
        var left = Success(value);
        var rightException = new Exception("Right exception");
        var right = Result.Error(rightException);

        // Act
        var result = left + right;

        // Assert
        result.Should().NotBeSameAs(left);
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(rightException);
    }
}
