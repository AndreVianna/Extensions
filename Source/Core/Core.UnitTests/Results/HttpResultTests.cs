using static DotNetToolbox.Results.HttpResult;

namespace DotNetToolbox.Results;

public class HttpResultTests {
    private static readonly HttpResult _ok = Ok();
    private static readonly HttpResult _created = Created();
    private static readonly HttpResult _unauthorized = Unauthorized();
    private static readonly HttpResult _notFound = NotFound();
    private static readonly HttpResult _conflict = Conflict();
    private static readonly HttpResult _badRequest = BadRequest(new ValidationError("Some error.", "Source"));
    private static readonly HttpResult _badRequestWithSameError = BadRequest(new ValidationError("Some error.", "Source"));
    private static readonly HttpResult _badRequestWithOtherError = BadRequest(new ValidationError("Other error.", "Source"));
    private static readonly HttpResult _failure = InternalError(new Exception("Some error."));

    private static readonly HttpResult<string> _okWithValue = Ok("Value");
    private static readonly HttpResult<string> _createdWithValue = Created("Value");
    private static readonly HttpResult<string> _unauthorizedWithValue = Unauthorized<string>();
    private static readonly HttpResult<string> _notFoundWithValue = NotFound<string>();
    private static readonly HttpResult<string> _conflictWithValue = Conflict("Value");
    private static readonly HttpResult<string> _badRequestWithValue = BadRequest("Value", new ValidationError("Some error.", "Source"));
    private static readonly HttpResult<string> _failureWithValue = InternalError<string>(new Exception("Some error."));

    [Fact]
    public void ImplicitConversion_FromValidationError_ReturnsFailure() {
        // Act
        HttpResult result = new ValidationError("Some error.", nameof(result));

        // Assert
        result.IsOk.Should().BeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorArray_ReturnsFailure() {
        // Act
        HttpResult result = new[] { new ValidationError("Some error.", nameof(result)) };

        // Assert
        result.IsOk.Should().BeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorList_ReturnsFailure() {
        // Act
        HttpResult result = new List<ValidationError> { new("Some error.", nameof(result)) };

        // Assert
        result.IsOk.Should().BeFalse();
    }

    private static HttpResult? ToResult(string? result)
        => result switch {
            null => null,
            nameof(_ok) => _ok,
            nameof(_created) => _created,
            nameof(_unauthorized) => _unauthorized,
            nameof(_notFound) => _notFound,
            nameof(_conflict) => _conflict,
            nameof(_badRequest) => _badRequest,
            nameof(_badRequestWithSameError) => _badRequestWithSameError,
            nameof(_badRequestWithOtherError) => _badRequestWithOtherError,
            nameof(_failure) => _failure,
            nameof(_okWithValue) => _ok,
            nameof(_createdWithValue) => _created,
            nameof(_unauthorizedWithValue) => _unauthorized,
            nameof(_notFoundWithValue) => _notFound,
            nameof(_conflictWithValue) => _conflict,
            nameof(_badRequestWithValue) => _badRequest,
            nameof(_failureWithValue) => _failure,
            _ => throw new ArgumentException($"Invalid field name: {result}"),
        };

    private sealed class TestDataForProperties : TheoryData<string, bool, bool, bool, bool, bool, bool> {
        public TestDataForProperties() {
            Add(nameof(_badRequest), true, false, false, false, false, false);
            Add(nameof(_failure), false, false, false, false, false, false);
            Add(nameof(_ok), false, true, false, false, false, false);
            Add(nameof(_notFound), false, false, true, false, false, false);
            Add(nameof(_conflict), false, false, false, true, false, false);
            Add(nameof(_created), false, false, false, false, true, false);
            Add(nameof(_unauthorized), false, false, false, false, false, true);
            Add(nameof(_badRequestWithValue), true, false, false, false, false, false);
            Add(nameof(_failureWithValue), false, false, false, false, false, false);
            Add(nameof(_okWithValue), false, true, false, false, false, false);
            Add(nameof(_notFoundWithValue), false, false, true, false, false, false);
            Add(nameof(_conflictWithValue), false, false, false, true, false, false);
            Add(nameof(_createdWithValue), false, false, false, false, true, false);
            Add(nameof(_unauthorizedWithValue), false, false, false, false, false, true);
        }
    }
    [Theory]
    [ClassData(typeof(TestDataForProperties))]
    public void Properties_ShouldReturnAsExpected(string subject, bool isBadRequest, bool isOk, bool isNotFound, bool isConflict, bool isCreated, bool isUnauthorized) {
        // Assert
        ToResult(subject)!.IsBadRequest.Should().Be(isBadRequest);
        ToResult(subject)!.IsOk.Should().Be(isOk);
        ToResult(subject)!.WasNotFound.Should().Be(isNotFound);
        ToResult(subject)!.HasConflict.Should().Be(isConflict);
        ToResult(subject)!.WasCreated.Should().Be(isCreated);
        ToResult(subject)!.IsUnauthorized.Should().Be(isUnauthorized);
    }

    private sealed class TestDataForEquality : TheoryData<string, string?, bool> {
        public TestDataForEquality() {
            Add(nameof(_ok), null, false);
            Add(nameof(_ok), nameof(_ok), true);
            Add(nameof(_ok), nameof(_notFound), false);
            Add(nameof(_ok), nameof(_conflict), false);
            Add(nameof(_ok), nameof(_badRequest), false);
            Add(nameof(_notFound), null, false);
            Add(nameof(_notFound), nameof(_ok), false);
            Add(nameof(_notFound), nameof(_notFound), true);
            Add(nameof(_notFound), nameof(_conflict), false);
            Add(nameof(_notFound), nameof(_badRequest), false);
            Add(nameof(_conflict), null, false);
            Add(nameof(_conflict), nameof(_ok), false);
            Add(nameof(_conflict), nameof(_notFound), false);
            Add(nameof(_conflict), nameof(_conflict), true);
            Add(nameof(_conflict), nameof(_badRequest), false);
            Add(nameof(_badRequest), null, false);
            Add(nameof(_badRequest), nameof(_ok), false);
            Add(nameof(_badRequest), nameof(_notFound), false);
            Add(nameof(_badRequest), nameof(_conflict), false);
            Add(nameof(_badRequest), nameof(_badRequest), true);
            Add(nameof(_badRequest), nameof(_badRequestWithSameError), true);
            Add(nameof(_badRequest), nameof(_badRequestWithOtherError), false);
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
        var expectedResult = new HashSet<HttpResult> {
            _ok,
            _badRequest,
            _badRequestWithOtherError,
        };

        // Act
        var result = new HashSet<HttpResult> {
            Ok(),
            Ok(),
            _ok,
            _ok,
            _badRequest,
            _badRequest,
            _badRequestWithSameError,
            _badRequestWithOtherError,
        };

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void Ok_CreatesResult() {
        // Arrange & Act
        var result = Ok();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void BadRequest_WithMessageOnly_CreatesResult() {
        // Arrange & Act
        var result = BadRequest(new ValidationError("Some error."));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void BadRequest_WithSourceAndMessage_CreatesResult() {
        // Arrange & Act
        var result = BadRequest(new ValidationError("Some error.", "Field1"));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo(new[] {
            new ValidationError("Some error.", "Field1"),
        });
    }

    [Fact]
    public void BadRequest_WithResult_CreatesResult() {
        // Arrange & Act
        var result = BadRequest(Result.Invalid("Some error."));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void AddOperator_WithoutError_ReturnsBadRequest() {
        // Arrange
        var result = Ok();

        // Act
        result += Result.Success();

        // Assert
        result.IsOk.Should().BeTrue();
        result.IsBadRequest.Should().BeFalse();
    }

    [Fact]
    public void AddOperator_WithOtherError_ReturnsBothErrors() {
        // Arrange
        var result = BadRequest(new ValidationError("Some error.", "Source"));

        // Act
        result += new ValidationError("Source", "Other error.");

        // Assert
        result.IsOk.Should().BeFalse();
        result.Errors.Should().HaveCount(2);
    }

    [Fact]
    public void AddOperator_WithSameError_ReturnsOnlyOneError() {
        // Arrange
        var result = BadRequest(new ValidationError("Some error.", "Source"));

        // Act
        result += new ValidationError("Some error.", "Source");

        // Assert
        result.IsOk.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void OfT_CopyConstructor_ClonesObject() {
        // Arrange
        var original = Ok(42);

        // Act
        var result = original with {
            Value = 7,
        };

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(7);
    }

    [Fact]
    public void AddOperator_WithError_ReturnsBadRequest() {
        // Arrange
        var result = Ok("SomeToken");

        // Act
        result += new ValidationError("Some error.", "Source");

        // Assert
        result.IsOk.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromValue_ReturnsOk() {
        // Act
        HttpResult<string> subject = "Value";

        // Assert
        subject.Value.Should().Be("Value");
        subject.IsOk.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromOkResult_ReturnsOk() {
        // Act
        HttpResult<string> subject = Result.Success("Value");

        // Assert
        subject.Value.Should().Be("Value");
        subject.IsOk.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromBadRequestResult_ReturnsOk() {
        // Act
        var result = Result.Invalid("Value", "Some error.", "SomeProperty");
        HttpResult<string> subject = result;

        // Assert
        subject.Value.Should().Be("Value");
        subject.IsOk.Should().BeFalse();
        subject.Errors.Should().BeEquivalentTo(result.Errors);
    }

    [Fact]
    public void AddOperator_WithValueAndWithoutError_ReturnsBadRequest() {
        // Arrange
        var result = Ok("Value");

        // Act
        result += Result.Success();

        // Assert
        result.IsOk.Should().BeTrue();
        result.IsBadRequest.Should().BeFalse();
        result.Value.Should().Be("Value");
    }

    [Fact]
    public void AddOperator_WithValueAndWithError_ReturnsBadRequest() {
        // Arrange
        var result = Ok("Value");

        // Act
        result += new ValidationError("Some error.", "result");

        // Assert
        result.IsOk.Should().BeFalse();
        result.IsBadRequest.Should().BeTrue();
        result.Value.Should().Be("Value");
    }

    [Fact]
    public void MapTo_WithoutError_ReturnsOk() {
        // Arrange
        var subject = Ok("42");

        // Act
        var result = subject.MapTo(s => s is null ? default : int.Parse(s));

        // Assert
        result.Should().BeOfType<HttpResult<int>>();
        result.IsOk.Should().BeTrue();
    }

    [Fact]
    public void MapTo_FromNotFound_ReturnsOk() {
        // Arrange
        var subject = NotFound<string>();

        // Act
        var result = subject.MapTo(s => s is null ? default : int.Parse(s));

        // Assert
        result.Should().BeOfType<HttpResult<int>>();
        result.IsOk.Should().BeFalse();
        result.IsBadRequest.Should().BeFalse();
        result.WasNotFound.Should().BeTrue();
    }

    [Fact]
    public void MapTo_WithError_ReturnsBadRequest() {
        // Arrange
        var subject = BadRequest("42", new ValidationError("Some error.", "Field1"));

        // Act
        var result = subject.MapTo(s => s is null ? default : int.Parse(s));

        // Assert
        result.Should().BeOfType<HttpResult<int>>();
        result.IsOk.Should().BeFalse();
    }

    [Fact]
    public void OkOfT_CreatesResult() {
        // Arrange & Act
        var result = Ok(42);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void BadRequestOfT_WithMessageOnly_CreatesResult() {
        // Arrange & Act
        var result = BadRequest(42, new ValidationError("Some error."));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void BadRequestOfT_WithSourceAndMessage_CreatesResult() {
        // Arrange & Act
        var result = BadRequest(42, new ValidationError("Some error.", "Field1"));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo(new[] {
            new ValidationError("Some error.", "Field1"),
        });
    }

    [Fact]
    public void BadRequestOfT_WithResult_CreatesResult() {
        // Arrange & Act
        var result = BadRequest(42, Result.Invalid("Some error."));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task OkTask_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = OkTask();

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreatedTask_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = CreatedTask();

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UnauthorizedTask_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = UnauthorizedTask();

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeFalse();
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
    public async Task BadRequestTask_ReturnsTaskWithInvalidCrudResult() {
        // Act
        var task = BadRequestTask("Some error.");

        // Assert
        var result = await task;
        result.IsInvalid.Should().BeTrue();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task OkTaskTValue_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = OkTask("Test value");

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Test value");
    }

    [Fact]
    public async Task CreatedTaskTValue_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = CreatedTask("Test value");

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Test value");
    }

    [Fact]
    public async Task UnauthorizedTaskTValue_ReturnsTaskWithSuccessCrudResult() {
        // Act
        var task = UnauthorizedTask<string>();

        // Assert
        var result = await task;
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void MapTo_WhenMappingFunctionThrows_ResultsInErrorCrudResult() {
        // Arrange
        var result = Ok("42");
        static int MappingFunction(string _) => throw new InvalidOperationException();

        // Act
        var mappedResult = result.MapTo(MappingFunction!);

        // Assert
        mappedResult.Should().BeOfType<HttpResult<int>>();
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
        mappedResult.Should().BeOfType<HttpResult<int>>();
        mappedResult.WasNotFound.Should().BeTrue();
    }

    [Fact]
    public void Error_WithString_CreatesResultWithException() {
        // Arrange
        const string errorMessage = "ErrorWriter message";

        // Act
        var result = InternalError(errorMessage);

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
        var result = InternalError(exception);

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public async Task NotFoundTaskTValue_ReturnsTaskWithNotFoundHttpResult() {
        // Act
        var task = NotFoundTask<int>();

        // Assert
        var result = await task;
        result.WasNotFound.Should().BeTrue();
        result.Value.Should().Be(default);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorToHttpResultTValue_ReturnsBadRequest() {
        // Act
        HttpResult<int> result = new ValidationError("Some error.", nameof(result));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Value.Should().Be(default);
    }

    [Fact]
    public void AdditionOperator_CombiningHttpResultWithError_ReturnsResultWithError() {
        // Arrange
        var result = Ok();
        var error = new ValidationError("ErrorWriter message", "Property");

        // Act
        var combinedResult = result + error;

        // Assert
        combinedResult.HasErrors.Should().BeTrue();
        combinedResult.Errors.Should().ContainSingle(e => e.Message == "ErrorWriter message" && e.Source == "Property");
    }

    // ... similar tests for other operator overloads ...

    [Fact]
    public void MapTo_WithException_ReturnsHttpResultTNewValueWithSameException() {
        // Arrange
        var exception = new Exception("ErrorWriter message");
        var subject = InternalError<int>(exception);

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
        var result = Ok(expectedValue);

        // Assert
        result.Value.Should().Be(expectedValue);
    }

    // ... similar tests for other factory methods and implicit conversions that set Value ...

    [Fact]
    public void EnsureIsSuccess_WhenHasErrors_ThrowsValidationException() {
        // Arrange
        var result = BadRequest(new ValidationError("ErrorWriter message"));

        // Act & Assert
        var action = () => result.EnsureIsSuccess();

        action.Should().Throw<ValidationException>().WithMessage(ValidationException.DefaultMessage);
    }

    [Fact]
    public void EnsureIsSuccess_WhenHasException_ThrowsValidationException() {
        // Arrange
        var exception = new Exception("Original exception");
        var result = InternalError(exception);

        // Act & Assert
        var action = () => result.EnsureIsSuccess();

        action.Should().Throw<ValidationException>().WithInnerException<Exception>().WithMessage("Original exception");
    }

    [Fact]
    public void TypeProperty_WhenHasException_ReturnsError() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        var result = InternalError(exception);

        // Assert
        result.Type.Should().Be(HttpResultState.ServerError);
    }

    [Fact]
    public async Task InternalErrorTask_WithString_ReturnsTaskWithErrorHttpResult() {
        // Arrange
        const string errorMessage = "ErrorWriter message";

        // Act
        var task = InternalErrorTask(errorMessage);
        var result = await task;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception!.Message.Should().Be(errorMessage);
    }

    [Fact]
    public async Task InternalErrorTask_WithException_ReturnsTaskWithErrorHttpResult() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        var task = InternalErrorTask(exception);
        var result = await task;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromExceptionToHttpResult_CreatesResultWithException() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        HttpResult result = exception;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromStringToHttpResult_CreatesResultWithException() {
        // Act
        HttpResult result = "ErrorWriter message";

        // Assert
        result.HasException.Should().BeFalse();
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorsToHttpResult_CreatesResultWithErrors() {
        // Arrange
        var errors = new ValidationErrors {
            new ValidationError("ErrorWriter message", "Property"),
        };

        // Act
        HttpResult result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromHashSetValidationErrorToHttpResult_CreatesResultWithErrors() {
        // Arrange
        var errors = new HashSet<ValidationError> {
            new("ErrorWriter message", "Property"),
        };

        // Act
        HttpResult result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromHttpResultToValidationErrorArray_ReturnsErrors() {
        // Arrange
        var errors = new[] { new ValidationError("ErrorWriter message", "Property") };

        // Act
        ValidationErrors resultErrors = BadRequest(errors);
        ValidationError[] errorArray = BadRequest(errors);
        Exception? resultException = BadRequest(errors);

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
        ValidationErrors resultErrors = InternalError(exception);
        ValidationError[] errorArray = InternalError(exception);
        Exception? resultException = InternalError(exception);

        // Assert
        resultErrors.Should().BeEmpty();
        errorArray.Should().BeEmpty();
        resultException.Should().BeSameAs(exception);
    }

    [Fact]
    public void ErrorOfT_WithString_CreatesResultWithException() {
        // Arrange
        const string errorMessage = "ErrorWriter message";

        // Act
        var result = InternalError<int>(errorMessage);

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception!.Message.Should().Be(errorMessage);
    }

    [Fact]
    public async Task ConflictTaskTValue_ReturnsTaskWithConflictHttpResult() {
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
    public async Task BadRequestTaskTValue_ReturnsTaskWithInvalidHttpResult() {
        // Arrange
        const string value = "Test value";
        var errors = new[] { new ValidationError("ErrorWriter message", "Property") };
        var invalidResult = Result.Invalid(errors);

        // Act
        var task = BadRequestTask(value, invalidResult);
        var result = await task;

        // Assert
        result.IsInvalid.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public async Task InternalErrorTaskTValue_WithString_ReturnsTaskWithErrorHttpResult() {
        // Arrange
        const string errorMessage = "ErrorWriter message";

        // Act
        var task = InternalErrorTask<int>(errorMessage);
        var result = await task;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeOfType<Exception>();
        result.Exception!.Message.Should().Be(errorMessage);
    }

    [Fact]
    public async Task InternalErrorTaskTValue_WithException_ReturnsTaskWithErrorHttpResult() {
        // Arrange
        var exception = new Exception("ErrorWriter message");

        // Act
        var task = InternalErrorTask<int>(exception);
        var result = await task;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void AdditionOperator_LeftHasException_ReturnsLeft() {
        // Arrange
        var left = InternalError(new Exception("Left exception"));
        var right = Result.Success();

        // Act
        var result = left + right;

        // Assert
        result.Should().BeSameAs(left);
        result.HasException.Should().BeTrue();
        result.Exception!.Message.Should().Be("Left exception");
    }

    [Fact]
    public void AdditionOperator_RightHasException_ReturnsNewHttpResultWithRightException() {
        // Arrange
        var left = Ok();
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
        var left = BadRequest(leftErrors);
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
        var left = BadRequest(leftErrors);
        var right = Result.Invalid(rightErrors);

        // Act
        var result = left + right;

        // Assert
        result.HasException.Should().BeFalse();
        result.Errors.Should().ContainSingle(); // Only one instance of the shared error
        result.Errors.Should().Contain(sharedError);
    }

    [Fact]
    public void ImplicitConversion_FromExceptionToHttpResultTValue_CreatesResultWithException() {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        HttpResult<string> result = exception;

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(exception);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorArrayToHttpResultTValue_CreatesResultWithErrors() {
        // Arrange
        var errors = new[] { new ValidationError("ErrorWriter 1", "Property1"), new ValidationError("ErrorWriter 2", "Property2") };

        // Act
        HttpResult<string> result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorListToHttpResultTValue_CreatesResultWithErrors() {
        // Arrange
        var errors = new List<ValidationError> { new("ErrorWriter 1", "Property1"), new("ErrorWriter 2", "Property2") };

        // Act
        HttpResult<string> result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorHashSetToHttpResultTValue_CreatesResultWithErrors() {
        // Arrange
        var errors = new HashSet<ValidationError> { new("ErrorWriter 1", "Property1"), new("ErrorWriter 2", "Property2") };

        // Act
        HttpResult<string> result = errors;

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void ImplicitConversion_FromHttpResultTValueToValidationErrorArray_ReturnsErrors() {
        // Arrange
        var errors = new[] { new ValidationError("ErrorWriter 1", "Property1"), new ValidationError("ErrorWriter 2", "Property2") };
        var result = BadRequest("Value", errors);

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
    public void ImplicitConversion_FromHttpResultTValueToException_ReturnsException() {
        // Arrange
        var exception = new Exception("Test exception");
        var result = InternalError<string>(exception);

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
    public void ImplicitConversion_FromHttpResultTValueToTValue_ReturnsValue() {
        // Arrange
        const string value = "Test value";
        var result = Ok(value);

        // Act
        string? resultValue = result;

        // Assert
        resultValue.Should().Be(value);
    }

    [Fact]
    public void AdditionOperator_CombiningHttpResultTValueWithResult_ReturnsCombinedErrors() {
        // Arrange
        const string value = "Test value";
        var leftErrors = new[] { new ValidationError("Left error", "LeftProperty") };
        var rightErrors = new[] { new ValidationError("Right error", "RightProperty") };
        var left = BadRequest(value, leftErrors);
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
    public void AdditionOperator_CombiningHttpResultTValueWithResultWhenLeftHasException_ReturnsLeft() {
        // Arrange
        var leftException = new Exception("Left exception");
        var left = InternalError<string>(leftException);
        var right = Result.Success();

        // Act
        var result = left + right;

        // Assert
        result.Should().BeSameAs(left);
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeSameAs(leftException);
    }

    [Fact]
    public void AdditionOperator_CombiningHttpResultTValueWithResultWhenRightHasException_ReturnsNewHttpResultTValueWithRightException() {
        // Arrange
        const string value = "Test value";
        var left = Ok(value);
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
