using static DotNetToolbox.Results.CrudResult;

namespace DotNetToolbox.Results;

public class CrudResultTests {
    private static readonly CrudResult _success = Success();
    private static readonly CrudResult _notFound = NotFound();
    private static readonly CrudResult _conflict = Conflict();
    private static readonly CrudResult _invalid = Invalid(new ValidationError("Some error.", "Source"));
    private static readonly CrudResult _invalidWithSameError = Invalid(new ValidationError("Some error.", "Source"));
    private static readonly CrudResult _invalidWithWithOtherError = Invalid(new ValidationError("Other error.", "Source"));
    private static readonly CrudResult _failure = Error(new Exception("Some error."));

    private static readonly CrudResult<string> _successWithValue = Success("Value");
    private static readonly CrudResult<string> _notFoundWithValue = NotFound<string>();
    private static readonly CrudResult<string> _conflictWithValue = Conflict("Value");
    private static readonly CrudResult<string> _invalidWithValue = Invalid("Value", new ValidationError("Some error.", "Source"));
    private static readonly CrudResult<string> _failureWithValue = Error<string>(new Exception("Some error."));

    [Fact]
    public void ImplicitConversion_FromValidationError_ReturnsFailure() {
        // Act
        CrudResult result = new ValidationError("Some error.", nameof(result));

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorArray_ReturnsFailure() {
        // Act
        CrudResult result = new[] { new ValidationError("Some error.", nameof(result)) };

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorList_ReturnsFailure() {
        // Act
        CrudResult result = new List<ValidationError> { new("Some error.", nameof(result)) };

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    private static CrudResult? ToResult(string? result)
        => result switch {
            null => null,
            nameof(_success) => _success,
            nameof(_notFound) => _notFound,
            nameof(_conflict) => _conflict,
            nameof(_invalid) => _invalid,
            nameof(_invalidWithSameError) => _invalidWithSameError,
            nameof(_invalidWithWithOtherError) => _invalidWithWithOtherError,
            nameof(_failure) => _failure,
            nameof(_successWithValue) => _successWithValue,
            nameof(_notFoundWithValue) => _notFoundWithValue,
            nameof(_conflictWithValue) => _conflictWithValue,
            nameof(_invalidWithValue) => _invalidWithValue,
            nameof(_failureWithValue) => _failureWithValue,
            _ => throw new ArgumentException($"Invalid field name: {result}"),
        };

    private sealed class TestDataForProperties : TheoryData<string, bool, bool, bool> {
        public TestDataForProperties() {
            Add(nameof(_invalid), false, false, false);
            Add(nameof(_failure), false, false, false);
            Add(nameof(_success), true, false, false);
            Add(nameof(_notFound), false, true, false);
            Add(nameof(_conflict), false, false, true);
            Add(nameof(_invalidWithValue), false, false, false);
            Add(nameof(_failureWithValue), false, false, false);
            Add(nameof(_successWithValue), true, false, false);
            Add(nameof(_notFoundWithValue), false, true, false);
            Add(nameof(_conflictWithValue), false, false, true);
        }
    }
    [Theory]
    [ClassData(typeof(TestDataForProperties))]
    public void Properties_ShouldReturnAsExpected(string subject, bool isSuccess, bool isNotFound, bool isConflict) {
        // Assert
        ToResult(subject)!.IsSuccess.Should().Be(isSuccess);
        ToResult(subject)!.WasNotFound.Should().Be(isNotFound);
        ToResult(subject)!.HasConflict.Should().Be(isConflict);
    }

    private sealed class TestDataForEquality : TheoryData<string, string?, bool> {
        public TestDataForEquality() {
            Add(nameof(_success), null, false);
            Add(nameof(_success), nameof(_success), true);
            Add(nameof(_success), nameof(_notFound), false);
            Add(nameof(_success), nameof(_conflict), false);
            Add(nameof(_success), nameof(_invalid), false);
            Add(nameof(_notFound), null, false);
            Add(nameof(_notFound), nameof(_success), false);
            Add(nameof(_notFound), nameof(_notFound), true);
            Add(nameof(_notFound), nameof(_conflict), false);
            Add(nameof(_notFound), nameof(_invalid), false);
            Add(nameof(_conflict), null, false);
            Add(nameof(_conflict), nameof(_success), false);
            Add(nameof(_conflict), nameof(_notFound), false);
            Add(nameof(_conflict), nameof(_conflict), true);
            Add(nameof(_conflict), nameof(_invalid), false);
            Add(nameof(_invalid), null, false);
            Add(nameof(_invalid), nameof(_success), false);
            Add(nameof(_invalid), nameof(_notFound), false);
            Add(nameof(_invalid), nameof(_conflict), false);
            Add(nameof(_invalid), nameof(_invalid), true);
            Add(nameof(_invalid), nameof(_invalidWithSameError), true);
            Add(nameof(_invalid), nameof(_invalidWithWithOtherError), false);
        }
    }

    [Theory]
    [ClassData(typeof(TestDataForEquality))]
    public void Equals_ReturnsAsExpected(string subject, string? other, bool expectedResult) {
        // Act
        var result = ToResult(subject) == ToResult(other);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [ClassData(typeof(TestDataForEquality))]
    public void NotEquals_ReturnsAsExpected(string subject, string? other, bool expectedResult) {
        // Act
        var result = ToResult(subject) != ToResult(other);

        // Assert
        result.Should().Be(!expectedResult);
    }

    [Fact]
    public void GetHashCode_DifferentiatesAsExpected() {
        var expectedResult = new HashSet<CrudResult> {
            _success,
            _invalid,
            _invalidWithWithOtherError,
        };

        // Act
        var result = new HashSet<CrudResult> {
            Success(),
            Success(),
            _success,
            _success,
            _invalid,
            _invalid,
            _invalidWithSameError,
            _invalidWithWithOtherError,
        };

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void Success_CreatesResult() {
        // Arrange & Act
        var result = Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Invalid_WithMessageOnly_CreatesResult() {
        // Arrange & Act
        var result = Invalid(new ValidationError("Some error."));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void Invalid_WithSourceAndMessage_CreatesResult() {
        // Arrange & Act
        var result = Invalid(new ValidationError("Some error.", "Field1"));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo(new[] {
            new ValidationError("Some error.", "Field1"),
        });
    }

    [Fact]
    public void Invalid_WithResult_CreatesResult() {
        // Arrange & Act
        var result = Invalid(Result.Invalid("Some error."));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void AddOperator_WithoutError_ReturnsInvalid() {
        // Arrange
        var result = Success();

        // Act
        result += Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
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
    }

    [Fact]
    public void AddOperator_WithOtherError_ReturnsBothErrors() {
        // Arrange
        var result = Invalid(new ValidationError("Some error 42.", "Source"));

        // Act
        result += new ValidationError("Other error 42.", "Source");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void AddOperator_WithSameError_ReturnsOnlyOneError() {
        // Arrange
        var result = Invalid(new ValidationError("Some error.", "Source"));

        // Act
        result += new ValidationError("Some error.", "Source");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void OfT_CopyConstructor_ClonesObject() {
        // Arrange
        var original = Success(42);

        // Act
        var result = original with {
            Value = 7,
        };

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(7);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ReturnsSuccess() {
        // Act
        CrudResult<string> subject = "Value";

        // Assert
        subject.Value.Should().Be("Value");
        subject.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromSuccessResult_ReturnsSuccess() {
        // Act
        var result = Result.Success("Value");
        CrudResult<string> subject = result;

        // Assert
        subject.Value.Should().Be(result.Value);
        subject.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromInvalidResult_ReturnsSuccess() {
        // Arrange
        var result = Invalid("Some error.");

        // Act
        ValidationErrors errors = result;
        ValidationError[] errorArray = result;
        Exception exception = result!;

        // Assert
        errors.Should().ContainSingle();
        errorArray.Should().ContainSingle();
        exception.Should().BeNull();
    }

    [Fact]
    public void ImplicitConversion_FromExceptionResult_ReturnsSuccess() {
        // Arrange
        var result = Error("Some error.");

        // Act
        ValidationErrors errors = result;
        ValidationError[] errorArray = result;
        Exception exception = result!;

        // Assert
        errors.Should().BeEmpty();
        errorArray.Should().BeEmpty();
        exception.Should().NotBeNull();
    }

    [Fact]
    public void AddOperator_WithValueAndWithoutError_ReturnsInvalid() {
        // Arrange
        var result = Success("Value");

        // Act
        result += Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Value");
    }

    [Fact]
    public void AddOperator_WithValueAndWithError_ReturnsInvalid() {
        // Arrange
        var result = Success("Value");

        // Act
        result += new ValidationError("Some error.", "result");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().Be("Value");
    }

    [Fact]
    public void MapTo_WithoutError_ReturnsSuccess() {
        // Arrange
        var subject = Success("42");

        // Act
        var result = subject.MapTo(s => s is null ? default : int.Parse(s));

        // Assert
        result.Should().BeOfType<CrudResult<int>>();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void MapTo_FromNotFound_ReturnsSuccess() {
        // Arrange
        var subject = NotFound<string>();

        // Act
        var result = subject.MapTo(s => s is null ? default : int.Parse(s));

        // Assert
        result.Should().BeOfType<CrudResult<int>>();
        result.IsSuccess.Should().BeFalse();
        result.WasNotFound.Should().BeTrue();
    }

    [Fact]
    public void MapTo_WithError_ReturnsInvalid() {
        // Arrange
        var subject = Invalid("42", new ValidationError("Some error.", "Field1"));

        // Act
        var result = subject.MapTo(s => s is null ? default : int.Parse(s));

        // Assert
        result.Should().BeOfType<CrudResult<int>>();
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void SuccessOfT_CreatesResult() {
        // Arrange & Act
        var result = Success(42);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void InvalidOfT_WithMessageOnly_CreatesResult() {
        // Arrange & Act
        var result = Invalid(42, new ValidationError("Some error."));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void InvalidOfT_WithSourceAndMessage_CreatesResult() {
        // Arrange & Act
        var result = Invalid(42, new ValidationError("Some error.", "Field1"));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo(new[] {
            new ValidationError("Some error.", "Field1"),
        });
    }

    [Fact]
    public void InvalidOfT_WithResult_CreatesResult() {
        // Arrange & Act
        var result = Invalid(42, Result.Invalid("Some error."));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromResult_SetsTypeBasedOnErrorPresence() {
        // Arrange
        var resultWithErrors = Result.Invalid("Test error");
        var resultWithoutErrors = Result.Success();

        // Act
        CrudResult crudResultWithErrors = resultWithErrors;
        CrudResult crudResultWithoutErrors = resultWithoutErrors;

        // Assert
        crudResultWithErrors.Type.Should().Be(CrudResultState.Invalid);
        crudResultWithoutErrors.Type.Should().Be(CrudResultState.Success);
    }

    [Fact]
    public void AdditionOperator_CombiningDifferentTypes_PreservesErrorType() {
        // Arrange
        var resultError = Error(new Exception("ErrorWriter"));
        var resultNotFound = Result.Success();

        // Act
        var combinedResult = resultError + resultNotFound;

        // Assert
        combinedResult.Type.Should().Be(CrudResultState.Error);
    }

    [Fact]
    public void AdditionOperator_CombiningCrudResultTValueInstances_PreservesValueType() {
        // Arrange
        var resultWithValue = Conflict("Test");
        var resultWithConflict = Result.Success();

        // Act
        var combinedResult = resultWithValue + resultWithConflict;

        // Assert
        combinedResult.Type.Should().Be(CrudResultState.Conflict);
        combinedResult.Value.Should().Be("Test");
    }

    [Fact]
    public void ResultTValue_Equals_WithDifferentTypes_ReturnsFalse() {
        // Arrange
        var resultSuccess = Success("Value1");
        var resultNotFound = NotFound();

        // Act
        var areEqual = resultSuccess.Equals(resultNotFound);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void ResultTValue_GetHashCode_WithDifferentTypes_ProducesDifferentHashCodes() {
        // Arrange
        var resultSuccess = Success("Value1");
        var resultNotFound = NotFound();

        // Act
        var hashCodeSuccess = resultSuccess.GetHashCode();
        var hashCodeNotFound = resultNotFound.GetHashCode();

        // Assert
        hashCodeSuccess.Should().NotBe(hashCodeNotFound);
    }

    [Fact]
    public async Task SuccessTask_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = SuccessTask();

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task NotFoundTask_ReturnsTaskWithNotFoundCrudResult() {
        // Act
        var task = NotFoundTask();

        // Assert
        var result = await task;
        result.WasNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task ConflictTask_ReturnsTaskWithConflictCrudResult() {
        // Act
        var task = ConflictTask();

        // Assert
        var result = await task;
        result.HasConflict.Should().BeTrue();
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
        var task = SuccessTask("Test value");

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Test value");
    }

    [Fact]
    public void MapTo_WhenMappingFunctionThrows_ResultsInErrorCrudResult() {
        // Arrange
        var result = Success("42");
        static int MappingFunction(string _) => throw new InvalidOperationException();

        // Act
        var mappedResult = result.MapTo(MappingFunction!);

        // Assert
        mappedResult.Should().BeOfType<CrudResult<int>>();
        mappedResult.HasException.Should().BeTrue();
        mappedResult.Exception.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void MapTo_WhenTypeIsNotFound_ReturnsNotFoundCrudResult() {
        // Arrange
        var result = NotFound<string>();

        // Act
        var mappedResult = result.MapTo(s => s!.Length);

        // Assert
        mappedResult.Should().BeOfType<CrudResult<int>>();
        mappedResult.WasNotFound.Should().BeTrue();
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
    public async Task NotFoundTaskTValue_ReturnsTaskWithNotFoundCrudResult() {
        // Act
        var task = NotFoundTask<int>();

        // Assert
        var result = await task;
        result.WasNotFound.Should().BeTrue();
        result.Value.Should().Be(default);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorToCrudResultTValue_ReturnsInvalid() {
        // Act
        CrudResult<int> result = new ValidationError("Some error.", nameof(result));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Value.Should().Be(default);
    }

    [Fact]
    public void AdditionOperator_CombiningCrudResultWithError_ReturnsResultWithError() {
        // Arrange
        var result = Success();
        var error = new ValidationError("ErrorWriter message", "Property");

        // Act
        var combinedResult = result + error;

        // Assert
        combinedResult.HasErrors.Should().BeTrue();
        combinedResult.Errors.Should().ContainSingle(e => e.Message == "ErrorWriter message" && e.Source == "Property");
    }

    // ... similar tests for other operator overloads ...

    [Fact]
    public void MapTo_WithException_ReturnsCrudResultTNewValueWithSameException() {
        // Arrange
        var exception = new Exception("ErrorWriter message");
        var subject = Error<int>(exception);

        // Act
        var result = subject.MapTo(value => value * 2);

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void ValueProperty_IsSetCorrectly() {
        // Arrange
        const string expectedValue = "Test value";

        // Act
        var result = Success(expectedValue);

        // Assert
        result.Value.Should().Be(expectedValue);
    }

    // ... similar tests for other factory methods and implicit conversions that set Value ...

    [Fact]
    public void EnsureIsSuccess_WhenHasErrors_ThrowsValidationException() {
        // Arrange
        var result = Invalid(new ValidationError("ErrorWriter message"));

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
        result.Type.Should().Be(CrudResultState.Error);
    }

    [Fact]
    public async Task ErrorTask_WithString_ReturnsTaskWithErrorCrudResult() {
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
    public async Task ErrorTask_WithException_ReturnsTaskWithErrorCrudResult() {
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
    public void ImplicitConversion_FromExceptionToCrudResult_CreatesResultWithException() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        CrudResult result = exception;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromStringToCrudResult_CreatesResultWithException() {
        // Act
        CrudResult result = "ErrorWriter message";

        // Assert
        result.HasException.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorsToCrudResult_CreatesResultWithErrors() {
        // Arrange
        var errors = new ValidationErrors {
            new ValidationError("ErrorWriter message", "Property"),
        };

        // Act
        CrudResult result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromHashSetValidationErrorToCrudResult_CreatesResultWithErrors() {
        // Arrange
        var errors = new HashSet<ValidationError> {
            new("ErrorWriter message", "Property"),
        };

        // Act
        CrudResult result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromCrudResultToValidationErrorArray_ReturnsErrors() {
        // Arrange
        var errors = new[] { new ValidationError("ErrorWriter message", "Property") };

        // Act
        ValidationError[] errorArray = Invalid(errors);

        // Assert
        errorArray.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromCrudResultToException_ReturnsException() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        Exception? resultException = Error(exception);

        // Assert
        resultException.Should().BeSameAs(exception);
    }

    [Fact]
    public void ErrorOfT_WithString_CreatesResultWithException() {
        // Arrange
        const string errorMessage = "ErrorWriter message";

        // Act
        var result = Error<int>(errorMessage);

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception!.Message.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ConflictTaskTValue_ReturnsTaskWithConflictCrudResult() {
        // Arrange
        const string value = "Test value";

        // Act
        var task = ConflictTask(value);
        var result = await task;

        // Assert
        result.HasConflict.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public async Task InvalidTaskTValue_ReturnsTaskWithInvalidCrudResult() {
        // Arrange
        const string value = "Test value";
        var errors = new[] { new ValidationError("ErrorWriter message", "Property") };
        var invalidResult = Result.Invalid(errors);

        // Act
        var task = InvalidTask(value, invalidResult);
        var result = await task;

        // Assert
        result.IsInvalid.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public async Task ErrorTaskTValue_WithString_ReturnsTaskWithErrorCrudResult() {
        // Arrange
        const string errorMessage = "ErrorWriter message";

        // Act
        var task = ErrorTask<int>(errorMessage);
        var result = await task;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception!.Message.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ErrorTaskTValue_WithException_ReturnsTaskWithErrorCrudResult() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        var task = ErrorTask<int>(exception);
        var result = await task;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
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
    public void AdditionOperator_RightHasException_ReturnsNewCrudResultWithRightException() {
        // Arrange
        var left = Success();
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
        var left = Invalid(leftErrors);
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
        var left = Invalid(leftErrors);
        var right = Result.Invalid(rightErrors);

        // Act
        var result = left + right;

        // Assert
        result.HasException.Should().BeFalse();
        result.Errors.Should().ContainSingle(); // Only one instance of the shared error
        result.Errors.Should().Contain(sharedError);
    }

    [Fact]
    public void ImplicitConversion_FromExceptionToCrudResultTValue_CreatesResultWithException() {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        CrudResult<string> result = exception;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorArrayToCrudResultTValue_CreatesResultWithErrors() {
        // Arrange
        var errors = new[] { new ValidationError("ErrorWriter 1", "Property1"), new ValidationError("ErrorWriter 2", "Property2") };

        // Act
        CrudResult<string> result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorListToCrudResultTValue_CreatesResultWithErrors() {
        // Arrange
        var errors = new List<ValidationError> { new("ErrorWriter 1", "Property1"), new("ErrorWriter 2", "Property2") };

        // Act
        CrudResult<string> result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorHashSetToCrudResultTValue_CreatesResultWithErrors() {
        // Arrange
        var errors = new HashSet<ValidationError> { new("ErrorWriter 1", "Property1"), new("ErrorWriter 2", "Property2") };

        // Act
        CrudResult<string> result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromCrudResultTValueToValidationErrorArray_ReturnsErrors() {
        // Arrange
        var errors = new[] { new ValidationError("ErrorWriter 1", "Property1"), new ValidationError("ErrorWriter 2", "Property2") };
        var result = Invalid("Value", errors);

        // Act
        ValidationErrors resultErrors = result;
        ValidationError[] errorArray = result;
        Exception? resultException = result;

        // Assert
        resultErrors.Should().BeEquivalentTo(errors);
        errorArray.Should().BeEquivalentTo(errors);
        resultException.Should().BeNull();
    }

    [Fact]
    public void ImplicitConversion_FromCrudResultTValueToException_ReturnsException() {
        // Arrange
        var exception = new Exception("Test exception");
        var result = Error<string>(exception);

        // Act
        ValidationErrors resultErrors = result;
        ValidationError[] errorArray = result;
        Exception? resultException = result;

        // Assert
        resultErrors.Should().BeEmpty();
        errorArray.Should().BeEmpty();
        resultException.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromCrudResultTValueToTValue_ReturnsValue() {
        // Arrange
        const string value = "Test value";
        var result = Success(value);

        // Act
        string? resultValue = result;

        // Assert
        resultValue.Should().Be(value);
    }

    [Fact]
    public void AdditionOperator_CombiningCrudResultTValueWithResult_ReturnsCombinedErrors() {
        // Arrange
        const string value = "Test value";
        var leftErrors = new[] { new ValidationError("Left error", "LeftProperty") };
        var rightErrors = new[] { new ValidationError("Right error", "RightProperty") };
        var left = Invalid(value, leftErrors);
        var right = Result.Invalid(rightErrors);

        // Act
        var result = left + right;

        // Assert
        result.HasException.Should().BeFalse();
        result.Value.Should().Be(value);
        result.Errors.Should().HaveCount(2);
        result.Errors.Should().Contain(leftErrors.Union(rightErrors));
    }

    [Fact]
    public void AdditionOperator_CombiningCrudResultTValueWithResultWhenLeftHasException_ReturnsLeft() {
        // Arrange
        var leftException = new Exception("Left exception");
        var left = Error<string>(leftException);
        var right = Result.Success();

        // Act
        var result = left + right;

        // Assert
        result.Should().BeSameAs(left);
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(leftException);
    }

    [Fact]
    public void AdditionOperator_CombiningCrudResultTValueWithResultWhenRightHasException_ReturnsNewCrudResultTValueWithRightException() {
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
