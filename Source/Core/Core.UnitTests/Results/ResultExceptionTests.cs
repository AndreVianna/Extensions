namespace DotNetToolbox.Results;

public class ResultExceptionTests {
    [Fact]
    public void Constructor_WithMessageOnly_CreatesException() {
        // Arrange & Act
        var exception = new ResultException("Some error.");

        // Assert
        exception.Errors.Should().ContainSingle();
        exception.Message.Should().Be("Some error.");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_CreatesException() {
        // Arrange & Act
        var exception = new ResultException("Some error.", new InvalidOperationException());

        // Assert
        exception.Errors.Should().ContainSingle();
        exception.Message.Should().Be("Some error.");
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithInnerException_CreatesException() {
        // Arrange
        var inner = new InvalidOperationException();

        // Act
        var exception = new ResultException(inner);

        // Assert
        exception.Errors.Should().ContainSingle();
        exception.Errors[0].Message.Should().Be("An error has occured.");
        exception.Message.Should().Be("An error has occured.");
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithSourceAndMessage_CreatesException() {
        // Arrange & Act
        var exception = new ResultException(new Error("Some error.", "Field1"));

        // Assert
        exception.Errors.Should().ContainSingle();
        exception.Message.Should().Be("Some error.");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithSourceAndMessageAndInnerException_CreatesException() {
        // Arrange & Act
        var exception = new ResultException(new Error("Some error.", "Field1"), new InvalidOperationException());

        // Assert
        exception.Errors.Should().ContainSingle();
        exception.Message.Should().Be("Some error.");
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithOneError_CreatesException() {
        // Arrange
        var error1 = new Error("Some error 42.");

        // Act
        var exception = new ResultException(error1);

        // Assert
        exception.Errors.Should().BeEquivalentTo([error1]);
        exception.Message.Should().Be("Validation failed.");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithOneErrorAndException_CreatesException() {
        // Arrange
        var error1 = new Error("Some error 42.");

        // Act
        var exception = new ResultException(error1, new InvalidOperationException());

        // Assert
        exception.Errors.Should().BeEquivalentTo([error1]);
        exception.Message.Should().Be("Validation failed.");
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithErrorCollection_CreatesException() {
        // Arrange
        var error1 = new Error("Some global error 42.");
        var error2 = new Error("Some other error 13.", "Field1");

        // Act
        var exception = new ResultException([error1, error2]);

        // Assert
        exception.Errors.Should().BeEquivalentTo([error1, error2]);
        exception.Message.Should().Be("Validation failed.");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithErrorCollectionAndException_CreatesException() {
        // Arrange
        var error1 = new Error("Some global error 42.");
        var error2 = new Error("Some other error 13.", "Field1");

        // Act
        var exception = new ResultException([error1, error2], new InvalidOperationException());

        // Assert
        exception.Errors.Should().BeEquivalentTo([error1, error2]);
        exception.Message.Should().Be("Validation failed.");
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithAllButSingleError_CreatesException() {
        // Arrange
        var error1 = new Error("Some global error 42.");

        // Act
        var exception = new ResultException("Some message.", error1);

        // Assert
        exception.Errors.Should().BeEquivalentTo([error1]);
        exception.Message.Should().Be("Some message.");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithAllButSingleErrorAndException_CreatesException() {
        // Arrange
        var error1 = new Error("Some global error 42.");

        // Act
        var exception = new ResultException("Some message.", error1, new InvalidOperationException());

        // Assert
        exception.Errors.Should().BeEquivalentTo([error1]);
        exception.Message.Should().Be("Some message.");
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithAllArguments_CreatesException() {
        // Arrange
        var error1 = new Error("Some global error 42.");
        var error2 = new Error("Some other error 13.", "Field1");

        // Act
        var exception = new ResultException("Some message.", [error1, error2], new InvalidOperationException());

        // Assert
        exception.Errors.Should().BeEquivalentTo([error1, error2]);
        exception.Message.Should().Be("Some message.");
        exception.InnerException.Should().NotBeNull();
    }
}
