using Errand.Abstractions;
using Errand.Core;
using Errand.Extensions;
using Errand.UnitTests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Errand.UnitTests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddErrand_ShouldRegisterIErrand()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrand(typeof(ServiceCollectionExtensionsTests));

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var errand = serviceProvider.GetService<IErrand>();
        errand.Should().NotBeNull();
        errand.Should().BeOfType<Errand.Core.Errand>();
    }

    [Fact]
    public void AddErrand_ShouldRegisterHandlersFromAssembly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrand(typeof(TestRequestHandler));

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var handler = serviceProvider.GetService<IRequestHandler<TestRequest, TestResponse>>();
        handler.Should().NotBeNull();
        handler.Should().BeOfType<TestRequestHandler>();
    }

    [Fact]
    public void AddErrand_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Act
        Action act = () => ServiceCollectionExtensions.AddErrand(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    [Fact]
    public void AddErrand_ShouldUseCallingAssembly_WhenNoAssembliesProvided()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrand();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var errand = serviceProvider.GetService<IErrand>();
        errand.Should().NotBeNull();
    }

    [Fact]
    public void AddErrand_ShouldRegisterHandlersWithSpecifiedLifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrand(ServiceLifetime.Singleton, typeof(TestRequestHandler).Assembly);

        // Assert
        var handlerDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IRequestHandler<TestRequest, TestResponse>));

        handlerDescriptor.Should().NotBeNull();
        handlerDescriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddErrand_WithMarkerTypes_ShouldRegisterHandlers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrand(typeof(TestRequestHandler));

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var handler = serviceProvider.GetService<IRequestHandler<TestRequest, TestResponse>>();
        handler.Should().NotBeNull();
    }

    [Fact]
    public void AddErrand_WithMarkerTypes_ShouldThrowException_WhenNoTypesProvided()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        Action act = () => services.AddErrand(Array.Empty<Type>());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*At least one assembly marker type*");
    }

    [Fact]
    public void AddErrandBehavior_ShouldRegisterBehavior()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrandBehavior<TestPipelineBehavior1>();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TestRequest, TestResponse>>();
        behaviors.Should().NotBeEmpty();
    }

    [Fact]
    public void AddErrandBehavior_ShouldRegisterOpenGenericBehavior()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        //services.AddErrandBehavior(typeof(TestOpenGenericBehavior<,>));

        // Assert
        var behaviorDescriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IPipelineBehavior<,>) &&
            d.ImplementationType == typeof(TestOpenGenericBehavior<,>));

        behaviorDescriptor.Should().NotBeNull();
    }

    [Fact]
    public void AddErrandBehaviors_ShouldRegisterBehaviorsFromAssembly()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrandBehaviors(typeof(TestPipelineBehavior1).Assembly);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TestRequest, TestResponse>>();
        behaviors.Should().NotBeEmpty();
    }

    [Fact]
    public void AddErrand_ShouldNotRegisterAbstractHandlers()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddErrand(typeof(AbstractTestHandler));

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var handler = serviceProvider.GetService<IRequestHandler<TestRequest, TestResponse>>();
        handler.Should().NotBeNull();
        handler.Should().NotBeOfType<AbstractTestHandler>();
    }

    [Fact]
    public void AddErrand_ShouldRegisterMultipleHandlersForSameRequest()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IRequestHandler<TestRequest, TestResponse>, TestRequestHandler>();
        services.AddTransient<IRequestHandler<TestRequest, TestResponse>, AlternativeTestRequestHandler>();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var handlers = serviceProvider.GetServices<IRequestHandler<TestRequest, TestResponse>>();

        // Assert
        handlers.Should().HaveCount(2);
    }
}