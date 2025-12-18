using Errand.Abstractions;
using Errand.Core;
using Errand.Exceptions;
using Errand.UnitTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Errand.UnitTests.Core;

public class ErrandTests
{
    [Fact]
    public async Task Send_ShouldCallHandler_WhenHandlerIsRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, TestResponse>, TestRequestHandler>();
        var serviceProvider = services.BuildServiceProvider();
        var errand = new Errand.Core.Errand(serviceProvider);
        var request = new TestRequest { Value = 42 };

        // Act
        var result = await errand.Send(request);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().Be(42);
    }

    [Fact]
    public async Task Send_ShouldThrowHandlerNotFoundException_WhenHandlerIsNotRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var errand = new Errand.Core.Errand(serviceProvider);
        var request = new TestRequest { Value = 42 };

        // Act
        Func<Task> act = async () => await errand.Send(request);

        // Assert
        await act.Should().ThrowAsync<HandlerNotFoundException>()
            .WithMessage("*No handler found*");
    }

    [Fact]
    public async Task Send_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var serviceProvider = services.BuildServiceProvider();
        var errand = new Errand.Core.Errand(serviceProvider);

        // Act
        Func<Task> act = async () => await errand.Send<TestResponse>(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenServiceProviderIsNull()
    {
        // Act
        Action act = () => new Errand.Core.Errand(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("serviceProvider");
    }

    [Fact]
    public async Task Send_ShouldCallHandlerMultipleTimes_WithDifferentRequests()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, TestResponse>, TestRequestHandler>();
        var serviceProvider = services.BuildServiceProvider();
        var errand = new Errand.Core.Errand(serviceProvider);

        // Act
        var result1 = await errand.Send(new TestRequest { Value = 10 });
        var result2 = await errand.Send(new TestRequest { Value = 20 });
        var result3 = await errand.Send(new TestRequest { Value = 30 });

        // Assert
        result1.Result.Should().Be(10);
        result2.Result.Should().Be(20);
        result3.Result.Should().Be(30);
    }

    [Fact]
    public async Task Send_ShouldHandleVoidRequests()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestVoidRequest, Unit>, TestVoidRequestHandler>();
        var serviceProvider = services.BuildServiceProvider();
        var errand = new Errand.Core.Errand(serviceProvider);
        var request = new TestVoidRequest();

        // Act
        var result = await errand.Send(request);

        // Assert
        result.Should().Be(Unit.Value);
        TestVoidRequestHandler.WasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Send_ShouldExecutePipelineBehaviors_InCorrectOrder()
    {
        // Arrange
        var executionOrder = new List<string>();
        var services = new ServiceCollection();

        services.AddTransient<IRequestHandler<TestRequest, TestResponse>, TestRequestHandler>();
        services.AddTransient<IPipelineBehavior<TestRequest, TestResponse>>(
            _ => new TestPipelineBehavior1(executionOrder));
        services.AddTransient<IPipelineBehavior<TestRequest, TestResponse>>(
            _ => new TestPipelineBehavior2(executionOrder));

        var serviceProvider = services.BuildServiceProvider();
        var errand = new Errand.Core.Errand(serviceProvider);
        var request = new TestRequest { Value = 42 };

        // Act
        var result = await errand.Send(request);

        // Assert
        executionOrder.Should().ContainInOrder(
            "Behavior2-Before",
            "Behavior1-Before",
            "Handler",
            "Behavior1-After",
            "Behavior2-After");
    }

    [Fact]
    public async Task Send_ShouldCacheHandlerWrapper_ForPerformance()
    {
        // Arrange
        var services = new ServiceCollection();
        var handlerCallCount = 0;
        services.AddTransient<IRequestHandler<TestRequest, TestResponse>>(
            _ => new TestRequestHandler(() => handlerCallCount++));

        var serviceProvider = services.BuildServiceProvider();
        var errand = new Errand.Core.Errand(serviceProvider);

        // Act
        await errand.Send(new TestRequest { Value = 1 });
        await errand.Send(new TestRequest { Value = 2 });
        await errand.Send(new TestRequest { Value = 3 });

        // Assert
        handlerCallCount.Should().Be(3); // Handler should be called 3 times
    }

    [Fact]
    public async Task Send_ShouldPropagateExceptionFromHandler()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, TestResponse>, ThrowingTestRequestHandler>();
        var serviceProvider = services.BuildServiceProvider();
        var errand = new Errand.Core.Errand(serviceProvider);
        var request = new TestRequest { Value = 42 };

        // Act
        Func<Task> act = async () => await errand.Send(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Test exception");
    }
}