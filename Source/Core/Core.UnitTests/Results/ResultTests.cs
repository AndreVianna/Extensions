namespace DotNetToolbox.Results;

public class ResultTests {
    private static readonly Result _success = Result.Success();
    private static readonly Result _invalid = Result.Invalid("Some error.");
    private static readonly Result _invalidWithSameError = Result.Invalid("Some error.");
    private static readonly Result _invalidWithOtherError = Result.Invalid("Other error.");
    private static readonly Result _failure = Result.Error("Some error.");

    private static readonly Result<string> _successWithValue = Result.Success("42");
    private static readonly Result<string> _invalidWithValue = Result.Invalid<string>("42", "Some error.");
    private static readonly Result<string> _failureWithValue = Result.Error<string>("Some error.");

    [Fact]
    public void Success_CreatesSuccess() {
        // Act
        var result = _success with { };

        // Assert
        result.Type.Should().Be(ResultType.Success);
        result.IsSuccessful.Should().BeTrue();
        result.IsInvalid.Should().BeFalse();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ImplicitConversion_FromValidationError_ReturnsFailure() {
        // Act
        Result result = new ValidationError("Some error.", nameof(result));

        // Assert
        result.Type.Should().Be(ResultType.Invalid);
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorArray_ReturnsFailure() {
        // Act
        Result result = new[] { new ValidationError("Some error.", nameof(result)) };

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorList_ReturnsFailure() {
        // Act
        Result result = new List<ValidationError> { new("Some error.", nameof(result)) };

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorSet_ReturnsFailure() {
        // Act
        Result result = new HashSet<ValidationError> { new("Some error.", nameof(result)) };

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromException_ReturnsError() {
        // Act
        Result result = new InvalidOperationException("Some error.");

        // Assert
        result.Type.Should().Be(ResultType.Error);
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsFaulty.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ImplicitConversion_FromSuccess_ReturnsError() {
        // Act
        Exception? exception = _success;
        ValidationErrors errors = _success;

        // Assert
        exception.Should().BeNull();
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ImplicitConversion_FromFailure_ReturnsError() {
        // Act
        ValidationErrors errors = _failure;
        ValidationError[] array = _failure;
        Exception? exception = _failure;

        // Assert
        array.Should().BeEmpty();
        errors.Should().BeEmpty();
        exception.Should().NotBeNull();
    }

    [Fact]
    public void ImplicitConversion_FromInvalid_ReturnsError() {
        // Act
        ValidationErrors errors = _invalid;
        ValidationError[] array = _invalid;
        Exception? exception = _invalid;

        // Assert
        array.Should().NotBeEmpty();
        errors.Should().NotBeEmpty();
        exception.Should().BeNull();
    }

    [Fact]
    public void AddOperator_FromSuccess_WithInvalid_ReturnsException() {
        // Arrange
        var result = Result.Success();

        // Act
        result += Result.Success() + new ValidationError("Some error.", "result");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
    }

    [Fact]
    public void AddOperator_FromError_WithInvalid_ReturnsInvalid() {
        // Arrange
        var result = Result.Error(new Exception("ErrorWriter"));

        // Act
        result += new ValidationError("Some error.", "result");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsFaulty.Should().BeTrue();
    }

    [Fact]
    public void AddOperator_FromSuccess_WithError_ReturnsException() {
        // Arrange
        var result = Result.Success();

        // Act
        result += new Exception("Some error.");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsFaulty.Should().BeTrue();
    }

    private static Result? ToResult(string? result)
        => result switch {
            null => null,
            nameof(_success) => _success,
            nameof(_invalid) => _invalid,
            nameof(_invalidWithSameError) => _invalidWithSameError,
            nameof(_invalidWithOtherError) => _invalidWithOtherError,
            nameof(_failure) => _failure,
            _ => throw new ArgumentException($"Invalid field name: {result}"),
        };

    private sealed class TestDataForEquality : TheoryData<string, string?, bool> {
        public TestDataForEquality() {
            Add(nameof(_success), null, false);
            Add(nameof(_success), nameof(_success), true);
            Add(nameof(_success), nameof(_invalid), false);
            Add(nameof(_invalid), null, false);
            Add(nameof(_invalid), nameof(_success), false);
            Add(nameof(_invalid), nameof(_invalid), true);
            Add(nameof(_invalid), nameof(_invalidWithSameError), true);
            Add(nameof(_invalid), nameof(_invalidWithOtherError), false);
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
        var expectedResult = new HashSet<Result> {
            _success,
            _invalid,
            _invalidWithOtherError,
            _failure,
        };

        // Act
        var result = new HashSet<Result> {
            Result.Success(),
            Result.Success(),
            _success,
            _success,
            _invalid,
            _invalid,
            _invalidWithSameError,
            _invalidWithOtherError,
            _failure,
        };

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public void ImplicitConversion_FromValue_ReturnsSuccess() {
        // Act
        Result<string> subject = "Value";

        // Assert
        subject.Value.Should().Be("Value");
        subject.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void AddOperator_WithValueAndWithoutError_ReturnsInvalid() {
        // Arrange
        var result = _successWithValue;

        // Act
        result += Result.Success();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("42");
    }

    [Fact]
    public void AddOperator_WithValueAndWithError_ReturnsInvalid() {
        // Arrange
        var result = _successWithValue;

        // Act
        result += new ValidationError("Some error.", "result");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Value.Should().Be("42");
    }

    [Fact]
    public void MapTo_WithoutError_ReturnsSuccess() {
        // Arrange & Act
        var result = _successWithValue.MapTo(int.Parse);

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.IsSuccessful.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void MapTo_WithError_ReturnsInvalid() {
        // Arrange & Act
        var result = _invalidWithValue.MapTo(int.Parse);

        // Assert
        result.Should().BeOfType<Result<int>>();
        result.IsSuccessful.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void MapTo_WithException_ReturnsError() {
        // Arrange & Act
        var result = _failureWithValue.MapTo(int.Parse);

        // Assert
        result.Should().BeOfType<Result<int?>>();
        result.IsSuccessful.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.IsFaulty.Should().BeTrue();
    }

    [Fact]
    public void SuccessWithValue_CreatesSuccess() {
        // Act
        var result = _successWithValue with { };

        // Assert
        result.Type.Should().Be(ResultType.Success);
        result.IsSuccessful.Should().BeTrue();
        result.IsInvalid.Should().BeFalse();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Value.Should().Be("42");
    }

    [Fact]
    public void ImplicitConversion_FromString_ReturnsInvalidResultWithSingleError() {
        // Act
        Result result = "Test error";

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Test error");
    }

    [Fact]
    public void ImplicitConversion_WithValue_FromValidationError_ReturnsFailure() {
        // Act
        Result<string> result = new ValidationError("Some error.", nameof(result));

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_WithValue_FromValidationErrorArray_ReturnsFailure() {
        // Act
        Result<string> result = new[] { new ValidationError("Some error.", nameof(result)) };

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_WithValue_FromValidationErrorList_ReturnsFailure() {
        // Act
        Result<string> result = new List<ValidationError> { new("Some error.", nameof(result)) };

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_WithValue_FromValidationErrorSet_ReturnsFailure() {
        // Act
        Result<string> result = new HashSet<ValidationError> { new("Some error.", nameof(result)) };

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_WithValue_FromException_ReturnsError() {
        // Act
        Result<string> result = new InvalidOperationException("Some error.");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsFaulty.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ImplicitConversion_FromSuccessWithValue_ReturnsError() {
        // Act
        Exception? exception = _successWithValue;
        ValidationErrors errors = _successWithValue;
        string value = _successWithValue;

        // Assert
        exception.Should().BeNull();
        errors.Should().BeEmpty();
        value.Should().Be("42");
    }

    [Fact]
    public void ImplicitConversion_FromFailureWithValue_ReturnsError() {
        // Act
        Exception? exception = _failureWithValue;
        ValidationErrors errors = _failureWithValue;
        ValidationError[] array = _failureWithValue;
        string value = _failureWithValue;

        // Assert
        exception.Should().NotBeNull();
        array.Should().BeEmpty();
        errors.Should().BeEmpty();
        value.Should().BeNull();
    }

    [Fact]
    public void ImplicitConversion_FromInvalidWithValue_ReturnsError() {
        // Act
        Exception? exception = _invalidWithValue;
        ValidationErrors errors = _invalidWithValue;
        ValidationError[] array = _invalidWithValue;
        string value = _invalidWithValue;

        // Assert
        exception.Should().BeNull();
        array.Should().NotBeEmpty();
        errors.Should().NotBeEmpty();
        value.Should().Be("42");
    }

    [Fact]
    public void EnsureIsSuccess_WhenResultHasException_ThrowsValidationException() {
        // Arrange
        var result = Result.Error("Test exception.");

        // Act & Assert
        var action = new Action(() => result.EnsureIsSuccess());

        action.Should().Throw<ValidationException>().WithMessage(ValidationException.DefaultMessage);
    }

    [Fact]
    public void EnsureIsSuccess_WhenResultHasErrors_ThrowsValidationException() {
        // Arrange
        var result = Result.Invalid("Test error");

        // Act & Assert
        var action = new Action(() => result.EnsureIsSuccess());

        action.Should().Throw<ValidationException>().WithMessage(ValidationException.DefaultMessage);
    }

    [Fact]
    public void AddOperator_WithValue_FromSuccess_WithInvalid_ReturnsException() {
        // Arrange
        var result = Result.Success("42");

        // Act
        result += Result.Success() + new ValidationError("Some error.", "result");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeTrue();
        result.IsFaulty.Should().BeFalse();
        result.Value.Should().Be("42");
    }

    [Fact]
    public void AddOperator_WithValue_FromError_WithInvalid_ReturnsInvalid() {
        // Arrange
        var result = Result.Error<string>(new Exception("ErrorWriter"));

        // Act
        result += new ValidationError("Some error.", "result");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsFaulty.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void AddOperator_WithValue_FromSuccess_WithError_ReturnsException() {
        // Arrange
        var result = Result.Success("42");

        // Act
        result += new Exception("Some error.");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsInvalid.Should().BeFalse();
        result.IsFaulty.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void SuccessTValue_WithValue_SetsValueProperty() {
        // Arrange
        const string value = "Test value";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void InvalidTValue_WithValue_SetsValueAndErrors() {
        // Arrange
        const string value = "Test value";
        const string message = "Test error";

        // Act
        var result = Result.Invalid<string>(value, message);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Errors.Should().ContainSingle(e => e.Message == message);
    }

    [Fact]
    public void ErrorTValue_WithValue_SetsExceptionProperty() {
        // Arrange
        var exception = new Exception("Test exception");

        // Act
        var result = Result.Error<string>(exception);

        // Assert
        result.HasException.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void MapTo_WhenMappingFunctionThrows_ResultsInErrorResult() {
        // Arrange
        var result = Result.Success("42");
        static int MappingFunction(string _) => throw new InvalidOperationException();

        // Act
        var mappedResult = result.MapTo(MappingFunction);

        // Assert
        mappedResult.Should().BeOfType<Result<int>>();
        mappedResult.HasException.Should().BeTrue();
        mappedResult.Exception.Should().BeOfType<InvalidOperationException>();
    }

    // Additional tests for task-based factory methods
    [Fact]
    public async Task SuccessTask_ReturnsTaskWithSuccessResult() {
        // Act
        var task = Result.Task.FromResult(Success());

        // Assert
        var result = await task;
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task InvalidTask_ReturnsTaskWithInvalidResult() {
        // Act
        var task = Result.InvalidTask("Test error");

        // Assert
        var result = await task;
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Test error");
    }

    [Fact]
    public async Task InvalidTask_FromResult_ReturnsTaskWithInvalidResult() {
        // Arrange
        var original = Result.Invalid("Test error");

        // Act
        var task = Result.InvalidTask(original);

        // Assert
        var result = await task;
        result.HasErrors.Should()
              .BeTrue();
        result.Errors.Should()
              .ContainSingle(e => e.Message == "Test error");
    }

    [Fact]
    public async Task InvalidTask_FromErrors_ReturnsTaskWithInvalidResult() {
        // Arrange
        var original = Result.Invalid("Test error");

        // Act
        var task = Result.InvalidTask(original.Errors);

        // Assert
        var result = await task;
        result.HasErrors.Should()
              .BeTrue();
        result.Errors.Should()
              .ContainSingle(e => e.Message == "Test error");
    }

    [Fact]
    public async Task ErrorTask_ReturnsTaskWithErrorResult() {
        // Act
        var task = Result.ErrorTask("Test exception");

        // Assert
        var result = await task;
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeOfType<Exception>().Which.Message.Should().Be("Test exception");
    }
    [Fact]
    public void Invalid_WithoutParameters_ReturnsInvalidResultWithDefaultError() {
        // Act
        var result = Result.Invalid<string>("Some value.", "Some error.");

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Error_WithoutParameters_ReturnsErrorResultWithDefaultException() {
        // Act
        var result = Result.Error<string>("Some exception.");

        // Assert
        result.HasException.Should().BeTrue();
        result.Exception.Should().NotBeNull();
    }

    [Fact]
    public void ResultTValue_Equals_WithDifferentResults_ReturnsFalse() {
        // Arrange
        var resultWithValue1 = Result.Success("Value1");
        var resultWithValue2 = Result.Invalid("Some error", "Value1");

        // Act
        var areEqual = resultWithValue1.Equals(resultWithValue2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void ResultTValue_Equals_WithDifferentValues_ReturnsFalse() {
        // Arrange
        var resultWithValue1 = Result.Success("Value1");
        var resultWithValue2 = Result.Success("Value2");

        // Act
        var areEqual = resultWithValue1.Equals(resultWithValue2);

        // Assert
        areEqual.Should().BeFalse();
    }

    [Fact]
    public void ResultTValue_Equals_WithSameValues_ReturnsTrue() {
        // Arrange
        var resultWithValue1 = Result.Success("Value1");
        var resultWithValue2 = Result.Success("Value1");

        // Act
        var areEqual = resultWithValue1.Equals(resultWithValue2);

        // Assert
        areEqual.Should().BeTrue();
    }

    [Fact]
    public void ResultTValue_GetHashCode_WithDifferentValues_ProducesDifferentHashCodes() {
        // Arrange
        var resultWithValue1 = Result.Success("Value1");
        var resultWithValue2 = Result.Success("Value2");

        // Act
        var hashCode1 = resultWithValue1.GetHashCode();
        var hashCode2 = resultWithValue2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    // Task-based factory methods tests for Result<TValue>
    [Fact]
    public async Task SuccessTaskTValue_ReturnsTaskWithSuccessResult() {
        // Act
        var task = Result.SuccessTask("Test value");

        // Assert
        var result = await task;
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("Test value");
    }

    [Fact]
    public async Task InvalidTaskTValue_WithMessageAndSource_ReturnsTaskWithInvalidResult() {
        // Act
        var task = Result.InvalidTask("Test value", "Test error", "Test source");

        // Assert
        var result = await task;
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Test error" && e.Source == "Test source");
        result.Value.Should().Be("Test value");
    }

    [Fact]
    public async Task InvalidTaskTValue_WithResult_ReturnsTaskWithInvalidResult() {
        // Arrange
        var existingResult = Result.Invalid("Existing error");

        // Act
        var task = Result.InvalidTask("Test value", existingResult);

        // Assert
        var result = await task;
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Existing error");
        result.Value.Should().Be("Test value");
    }

    [Fact]
    public async Task ErrorTaskTValue_WithMessage_ReturnsTaskWithErrorResult() {
        // Act
        var task = Result.ErrorTask<string>("Test exception");

        // Assert
        var result = await task;
        result.HasException.Should().BeTrue();
        result.Exception.Should().BeOfType<Exception>().Which.Message.Should().Be("Test exception");
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task ErrorTaskTValue_WithException_ReturnsTaskWithErrorResult() {
        // Arrange
        var exception = new InvalidOperationException("Test exception");

        // Act
        var task = Result.ErrorTask<string>(exception);

        // Assert
        var result = await task;
        result.HasException.Should().BeTrue();
        result.Exception.Should().Be(exception);
    }
}
