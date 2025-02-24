namespace DotNetToolbox.Results;

public class ErrorTests {
    [Fact]
    public void WithSourceAndMessage_ReturnsFormattedMessage() {
        // Arrange
        var error1 = new Error("Some message.", "Field");

        // Act
        var error2 = error1 with { };

        // Assert
        error2.Should().NotBeSameAs(error1);
        error1.Message.Should().Be("Some message.");
        error1.Sources.Should().BeEquivalentTo("Field");
        error1.ToString().Should().Be("Field: Some message.");
    }

    [Fact]
    public void FormattedMessage_WithoutSource_ReturnsMessage() {
        // Act
        var error = new Error("Some message.");

        // Assert
        error.Sources.Should().BeEmpty();
        error.Message.Should().Be("Some message.");
    }

    [Fact]
    public void Equality_ShouldReturnAsExpected() {
        var subject = new Error("Break message data", "field");
        var same = new Error("Break message data", "field");
        var otherSource = new Error("Break message data", "otherField");
        var otherMessage = new Error("Other message data", "field");

        //Act
        var resultForNull = subject == null!;
        var resultForOtherSource = subject != otherSource;
        var resultForOtherTemplate = subject != otherMessage;
        var resultForSame = subject == same;

        //Assert
        resultForNull.Should().BeFalse();
        resultForOtherSource.Should().BeTrue();
        resultForOtherTemplate.Should().BeTrue();
        resultForSame.Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ShouldReturnAsExpected() {
        // Arrange & Act
        var errorSet = new HashSet<Error> {
            new("Source 1", "Some message 1 42."),
            new("Source 1", "Some message 1 42."),
            new(" Source 1 ", "Some message 1 42."),
            new("Source 1", "Some message 1 42."),
            new("Source 2", "Some message 1 42."),
            new("Source 1", "Some message 2 42."),
            new("Source 1", "Some message 1 7."),
        };

        // Assert
        errorSet.Should().BeEquivalentTo(new Error[] {
            new("Source 1", "Some message 1 42."),
            new("Source 2", "Some message 1 42."),
            new("Source 1", "Some message 2 42."),
            new("Source 1", "Some message 1 7."),
        });
    }
}
