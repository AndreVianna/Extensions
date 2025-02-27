namespace DotNetToolbox.Results;

public class ResultTests {
    private static readonly Result _success = Result.Success();
    private static readonly Result _invalid = Result.Failure("Some error.");
    private static readonly Result _invalidWithSameError = Result.Failure("Some error.");
    private static readonly Result _invalidWithOtherError = Result.Failure("Other error.");

    private static readonly Result<string> _successWithValue = Result.Success("42");
    private static readonly Result<string> _invalidWithValue = Result.Failure<string>("42", "Some error.");

    [Fact]
    public void Success_CreatesSuccess() {
        // Act
        var result = _success with { };

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ImplicitConversion_FromValidationError_ReturnsFailure() {
        // Act
        Result result = new Error("Some error.", nameof(result));

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorArray_ReturnsFailure() {
        // Act
        Result result = new[] { new Error("Some error.", nameof(result)) };

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorList_ReturnsFailure() {
        // Act
        Result result = new List<Error> { new("Some error.", nameof(result)) };

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void ImplicitConversion_FromValidationErrorSet_ReturnsFailure() {
        // Act
        Result result = new HashSet<Error> { new("Some error.", nameof(result)) };

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public void AddOperator_FromSuccess_WithInvalid_ReturnsException() {
        // Arrange
        var result = Result.Success();

        // Act
        result += Result.Success() + new Error("Some error.", "result");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
    }

    private static Result? ToResult(string? result)
        => result switch {
            null => null,
            nameof(_success) => _success,
            nameof(_invalid) => _invalid,
            nameof(_invalidWithSameError) => _invalidWithSameError,
            nameof(_invalidWithOtherError) => _invalidWithOtherError,
            _ => throw new ArgumentException($"Failure field name: {result}"),
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
        result += new Error("Some error.", "result");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Value.Should().Be("42");
    }

    [Fact]
    public void SuccessWithValue_CreatesSuccess() {
        // Act
        var result = _successWithValue with { };

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Errors.Should().BeEmpty();
        result.Value.Should().Be("42");
    }

    [Fact]
    public void ImplicitConversion_FromString_ReturnsInvalidResultWithSingleError() {
        // Act
        Result result = (Error)"Test error";

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(static e => e.Message == "Test error");
    }

    [Fact]
    public void AddOperator_WithValue_FromSuccess_WithInvalid_ReturnsException() {
        // Arrange
        var result = Result.Success("42");

        // Act
        result += Result.Success() + new Error("Some error.", "result");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Value.Should().Be("42");
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
        var result = Result.Failure<string>(value, message);

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Value.Should().Be(value);
        result.Errors.Should().ContainSingle(static e => e.Message == message);
    }

    // Additional tests for task-based factory methods
    [Fact]
    public async Task SuccessTask_ReturnsTaskWithSuccessResult() {
        // Act
        var task = Task.FromResult(Result.Success());

        // Assert
        var result = await task;
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task InvalidTask_ReturnsTaskWithInvalidResult() {
        // Act
        var task = Task.FromResult(Result.Failure("Test error"));

        // Assert
        var result = await task;
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(static e => e.Message == "Test error");
    }

    [Fact]
    public async Task InvalidTask_FromResult_ReturnsTaskWithInvalidResult() {
        // Arrange
        var original = Result.Failure("Test error");

        // Act
        var task = Task.FromResult(original);

        // Assert
        var result = await task;
        result.HasErrors.Should()
              .BeTrue();
        result.Errors.Should()
              .ContainSingle(static e => e.Message == "Test error");
    }

    [Fact]
    public async Task InvalidTask_FromErrors_ReturnsTaskWithInvalidResult() {
        // Arrange
        var original = Result.Failure("Test error");

        // Act
        var task = Task.FromResult(Result.Failure(original.Errors));

        // Assert
        var result = await task;
        result.HasErrors.Should()
              .BeTrue();
        result.Errors.Should()
              .ContainSingle(static e => e.Message == "Test error");
    }

    [Fact]
    public void Invalid_WithoutParameters_ReturnsInvalidResultWithDefaultError() {
        // Act
        var result = Result.Failure("Some value.", "Some error.");

        // Assert
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void ResultTValue_Equals_WithDifferentResults_ReturnsFalse() {
        // Arrange
        var resultWithValue1 = Result.Success("Value1");
        var resultWithValue2 = Result.Failure("Some error", "Value1");

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
        var task = Task.FromResult(Result.Success("Test value"));

        // Assert
        var result = await task;
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("Test value");
    }

    [Fact]
    public async Task InvalidTaskTValue_WithMessageAndSource_ReturnsTaskWithInvalidResult() {
        // Act
        var task = Task.FromResult(Result.Failure("Test value", "Test error", "Test source"));

        // Assert
        var result = await task;
        result.HasErrors.Should().BeTrue();
        result.Errors.Should().ContainSingle(static e => e.Message == "Test error" && e.Sources.Contains("Test source"));
        result.Value.Should().Be("Test value");
    }
}
