using DotNetToolbox.Validation;

namespace DotNetToolbox.Utilities;

public class EnsureTests {
    [Fact]
    public void IsOfType_WhenArgumentIsOfWrongType_ThrowsArgumentNullException() {
        // Arrange
        const int input = 12;

        // Act
        var action = static () => IsOfType<string>(input);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Expected value to be of type 'String'. Found: 'Int32'. (Parameter 'input')");
    }

    [Fact]
    public void IsOfType_WhenArgumentIsOfRightType_ReturnsInput() {
        // Arrange
        const string input = "value";

        // Act
        var result = IsOfType<string>(input);

        // Assert
        result.Should().BeSameAs(input);
    }

    [Fact]
    public void NotNull_WhenArgumentIsNull_ThrowsArgumentNullException() {
        // Arrange
        const object? input = null;

        // Act
        var action = static () => IsNotNull(input);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'input')");
    }

    [Fact]
    public void NotNull_WhenArgumentIsNotNull_ReturnsInput() {
        // Arrange
        var input = new object();

        // Act
        var result = IsNotNull(input);

        // Assert
        result.Should().BeSameAs(input);
    }

    [Fact]
    public void DoesNotHaveValue_WhenArgumentIsNullableValueType_ThrowsArgumentNullException() {
        // Arrange
        int? input = null;

        // Act
        var action = () => IsNotNull(input);

        // Assert
        action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'input')");
    }

    [Fact]
    public void DoesNotHaveValue_WhenArgumentIsNullableValueType_ReturnsInput() {
        // Arrange
        int? input = 3;

        // Act
        var result = IsNotNull(input);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void NotNullOrEmpty_WhenStringIsNull_ThrowsArgumentException() {
        // Arrange
        string? input = null;

        // Act
        var action = () => IsNotNullOrEmpty(input);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Value cannot be null. (Parameter 'input')");
    }

    [Fact]
    public void NotNullOrEmpty_WhenStringIsEmpty_ThrowsArgumentException() {
        // Arrange
        var input = string.Empty;

        // Act
        var action = () => IsNotNullOrEmpty(input);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("The value cannot be an empty string. (Parameter 'input')");
    }

    [Fact]
    public void NotNullOrEmpty_WhenStringIsNotEmpty_ReturnsInput() {
        // Arrange
        const string input = "Hello";

        // Act
        var result = IsNotNullOrEmpty(input);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void NotNullOrWhiteSpace_WhenStringIsNull_ThrowsArgumentException() {
        // Arrange
        const string? input = null;

        // Act
        var action = static () => IsNotNullOrWhiteSpace(input);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("Value cannot be null. (Parameter 'input')");
    }

    [Fact]
    public void NotNullOrWhiteSpace_WhenStringIsEmpty_ThrowsArgumentException() {
        // Arrange
        var input = string.Empty;

        // Act
        var action = () => IsNotNullOrWhiteSpace(input);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'input')");
    }

    [Fact]
    public void NotNullOrWhiteSpace_WhenStringIsWhiteSpace_ThrowsArgumentException() {
        // Arrange
        var input = " ";

        // Act
        var action = () => IsNotNullOrWhiteSpace(input);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("The value cannot be an empty string or composed entirely of whitespace. (Parameter 'input')");
    }

    [Fact]
    public void NotNullOrWhiteSpace_WhenStringIsNotEmpty_ReturnsInput() {
        // Arrange
        const string input = "Hello";

        // Act
        var result = IsNotNullOrWhiteSpace(input);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void DoesNotHaveNull_WhenDoesNotHaveNull_ThrowsArgumentException() {
        // Arrange
        var input = new[] { default(int?) };

        // Act
        var action = () => ItemsAreNotNull(input);

        // Assert
        action.Should().Throw<OperationFailureException>().WithMessage("The collection cannot contain null element(s).");
        action.Should().Throw<OperationFailureException>().Which.Source.Should().Be(nameof(input));
    }

    [Fact]
    public void NotNullOrDoesNotHaveNull_WhenIsNotEmpty_ReturnsInput() {
        // Arrange
        var input = new[] { 1, 2, 3 };

        // Act
        var result = ItemsAreNotNull(input);

        // Assert
        result.Should().BeSameAs(input);
    }

    [Fact]
    public void NotNullOrEmpty_WhenIsNull_ThrowsArgumentException() {
        // Arrange
        const ICollection<int> input = default!;

        // Act
        var result = IsNotEmpty(input);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void NotNullOrEmpty_WhenIsEmpty_ThrowsArgumentException() {
        // Arrange
        var input = Array.Empty<int>();

        // Act
        var action = () => IsNotNullOrEmpty(input);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("The collection cannot be empty. (Parameter 'input')");
    }

    [Fact]
    public void NoEmpty_WhenIsEmpty_ThrowsArgumentException() {
        // Arrange
        var input = Array.Empty<int>();

        // Act
        var action = () => IsNotEmpty(input);

        // Assert
        action.Should().Throw<ArgumentException>().WithMessage("The collection cannot be empty. (Parameter 'input')");
    }

    [Fact]
    public void NotNullOrEmpty_WhenIsNotEmpty_ReturnsInput() {
        // Arrange
        var input = new[] { 1, 2, 3 };

        // Act
        var result = IsNotEmpty(input);

        // Assert
        result.Should().BeSameAs(input);
    }

    [Fact]
    public void DoesNotHaveNulls_WhenDoesNotHaveNull_ThrowsArgumentException() {
        // Arrange
        var input = new[] { default(int?) };

        // Act
        var action = () => ItemsAreNotNull(input);

        // Assert
        action.Should().Throw<OperationFailureException>().WithMessage("The collection cannot contain null element(s).");
        action.Should().Throw<OperationFailureException>().Which.Source.Should().Be(nameof(input));
    }

    [Fact]
    public void DoesNotHaveNulls_WhenValid_ReturnsSame() {
        // Arrange
        var input = new[] { "hello" };

        // Act
        var result = ItemsAreNotNull(input);

        // Assert
        result.Should().BeSameAs(input);
    }

    private sealed class ValidatableObject(bool isValid) : IValidatable {
        public Result Validate(IMap? context = null)
            => isValid ? Result.Success() : Result.Failure("Is not valid.", "Source");
    }

    [Fact]
    public void IsValid_WhenInvalidValidatable_ThrowsValidationException() {
        // Arrange
        var input = new ValidatableObject(isValid: false);

        // Act
        var result = () => IsValid(input);

        // Assert
        result.Should().Throw<OperationFailureException>();
    }

    [Fact]
    public void IsValid_WhenValidValidatable_ReturnsSame() {
        // Arrange
        var input = new ValidatableObject(isValid: true);

        // Act
        var result = IsValid(input);

        // Assert
        result.Should().BeSameAs(input);
    }

    [Fact]
    public void IsValid_WhenValidSimpleObject_ReturnsSame() {
        var input = new object();

        // Act
        var result = IsValid(input, static _ => Result.Success());

        // Assert
        result.Should().BeSameAs(input);
    }

    [Fact]
    public void IsValid_WhenInvalidSimpleObject_ReturnsSame() {
        // Arrange
        var input = new object();

        // Act
        var result = () => IsValid(input, _ => Result.Failure("Some error."));

        // Assert
        result.Should().Throw<OperationFailureException>();
    }

    [Fact]
    public void IsValid_WhenValidSimpleObject_AndPredicate_ReturnsSame() {
        // Arrange
        var input = new object();

        // Act
        var result = IsValid(input, static _ => true);

        // Assert
        result.Should().BeSameAs(input);
    }

    [Fact]
    public void IsValid_WhenInvalidSimpleObject_AndPredicate_ReturnsSame() {
        // Arrange
        var input = new object();

        // Act
        var result = () => IsValid(input, _ => false);

        // Assert
        result.Should().Throw<OperationFailureException>();
    }

    [Fact]
    public void IsNotNullOrDefault_WhenArgumentIsNull_ReturnsDefault() {
        // Arrange
        const string? argument = null;
        const string defaultValue = "default";

        // Act
        var result = DefaultIfNull(argument, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void IsNotNullOrDefault_WhenArgumentIsNotNull_ReturnsArgument() {
        // Arrange
        const string argument = "test";
        const string defaultValue = "default";

        // Act
        var result = DefaultIfNull(argument, defaultValue);

        // Assert
        result.Should().Be(argument);
    }

    [Fact]
    public void IsValidOrDefault_WhenValidatableIsValid_ReturnsArgument() {
        // Arrange
        var argument = For<IValidatable>();
        argument.Validate().Returns(Result.Success());

        // Act
        var result = DefaultIfNotValid(argument, defaultValue: argument);

        // Assert
        result.Should().Be(argument);
    }

    [Fact]
    public void IsValidOrDefault_WhenValidatableIsInvalid_ReturnsDefault() {
        // Arrange
        var argument = new ValidatableObject(false);
        var defaultValue = new ValidatableObject(true);

        // Act
        var result = DefaultIfNotValid(argument, defaultValue: defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void IsValidOrDefault_WhenValidatableIsNull_ReturnsDefault() {
        // Arrange
        var argument = default(IValidatable);
        var defaultValue = new ValidatableObject(true);

        // Act
        var result = DefaultIfNotValid(argument, defaultValue: defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void IsValidOrDefault_WhenArgumentIsValid_ReturnsArgument() {
        // Arrange
        const string argument = "Valid";

        // Act
        var result = DefaultIfNotValid(argument, static _ => Result.Success(), argument);

        // Assert
        result.Should().Be(argument);
    }

    [Fact]
    public void IsValidOrDefault_WhenArgumentIsInvalid_ReturnsDefault() {
        // Arrange
        const string argument = "Invalid";
        const string defaultValue = "Valid";

        // Act
        var result = DefaultIfNotValid(argument, static _ => Result.Failure("ErrorWriter"), defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void IsValidOrDefault_WhenArgumentIsNull_ReturnsDefault() {
        // Arrange
        const string argument = default!;
        const string defaultValue = "Valid";

        // Act
        var result = DefaultIfNotValid(argument, static _ => Result.Success(), defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void IsValidOrDefault_WhenIsValid_ReturnsArgument() {
        // Arrange
        const string argument = "Valid";

        // Act
        var result = DefaultIfNotValid(argument, static _ => true, argument);

        // Assert
        result.Should().Be(argument);
    }

    [Fact]
    public void IsValidOrDefault_WhenIsInvalid_ReturnsDefault() {
        // Arrange
        const string argument = "Invalid";
        const string defaultValue = "Valid";

        // Act
        var result = DefaultIfNotValid(argument, static _ => false, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void IsValidOrDefault_WhenIsNull_ReturnsDefault() {
        // Arrange
        const string argument = default!;
        const string defaultValue = "Valid";

        // Act
        var result = DefaultIfNotValid(argument, static _ => true, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void DoesNotContainInvalidItems_WhenAllAreValidatableAndValid_ReturnsArgument() {
        // Arrange
        var argument = new List<ValidatableObject> { default!, new(true), new(true) };

        // Act
        var result = ItemsAreValid(argument);

        // Assert
        result.Should().BeEquivalentTo(argument);
    }

    [Fact]
    public void DoesNotContainInvalidItems_WhenAllAreValidatableAndAnyIsInvalid_ThrowsValidationException() {
        // Arrange
        var argument = new List<ValidatableObject> { default!, new(true), new(false) };

        // Act
        Action act = () => ItemsAreValid(argument);

        // Assert
        act.Should().Throw<OperationFailureException>().WithMessage("The collection cannot have invalid element(s).");
        act.Should().Throw<OperationFailureException>().Which.Source.Should().Be(nameof(argument));
    }

    [Fact]
    public void DoesNotContainInvalidItems_WhenAllElementsAreValid_ReturnsArgument() {
        // Arrange
        var argument = new List<string> { default!, "Valid", "Valid" };

        // Act
        var result = ItemsAreValid<List<string>, string>(argument, static _ => Result.Success());

        // Assert
        result.Should().BeEquivalentTo(argument);
    }

    [Fact]
    public void DoesNotContainInvalidItems_WhenAnyElementIsInvalid_ThrowsValidationException() {
        // Arrange
        var argument = new List<string> { default!, "Valid", "Invalid" };

        // Act
        Action act = () => ItemsAreValid<List<string>, string>(argument, _ => Result.Failure("ErrorWriter"));

        // Assert
        act.Should().Throw<OperationFailureException>().WithMessage("The collection cannot have invalid element(s).");
        act.Should().Throw<OperationFailureException>().Which.Source.Should().Be(nameof(argument));
    }

    [Fact]
    public void DoesNotContainInvalidItems_WhenArgumentIsNull_ReturnsArgument() {
        // Act
        Action act = static () => IsValid<string[]>(default, static _ => true);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithMessage("*default*");
    }

    [Fact]
    public void DoesNotContainInvalidItems_WhenDoesNotContainInvalidItems_ReturnsArgument() {
        // Arrange
        string[] argument = [default!, "One", "Two"];

        // Act
        var result = IsValid(argument, static _ => true);

        // Assert
        result.Should().BeEquivalentTo(argument);
    }

    [Fact]
    public void DoesNotContainInvalidItems_WhenAnyIsInvalid_ThrowsValidationException() {
        // Arrange
        string[] argument = [default!, "One", "Invalid"];

        // Act
        Action act = () => IsValid(argument, _ => false);

        // Assert
        act.Should().Throw<OperationFailureException>().WithMessage("The value is not valid.");
        act.Should().Throw<OperationFailureException>().Which.Source.Should().Be(nameof(argument));
    }
}
