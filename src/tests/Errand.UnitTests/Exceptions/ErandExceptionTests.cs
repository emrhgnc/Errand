using Errand.Exceptions;
using FluentAssertions;
using Xunit;

namespace Errand.UnitTests.Exceptions;

public class ErrandExceptionTests
{
    [Fact]
    public void Constructor_Default_ShouldCreateException()
    {
        // Act
        var exception = new ErrandException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Test error message";

        // Act
        var exception = new ErrandException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        var message = "Test error message";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new ErrandException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}

public class HandlerNotFoundExceptionTests
{
    [Fact]
    public void Constructor_Default_ShouldCreateException()
    {
        // Act
        var exception = new HandlerNotFoundException();

        // Assert
        exception.Should().NotBeNull();
        exception.Should().BeAssignableTo<ErrandException>();
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        var message = "Handler not found for request";

        // Act
        var exception = new HandlerNotFoundException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        var message = "Handler not found for request";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new HandlerNotFoundException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}