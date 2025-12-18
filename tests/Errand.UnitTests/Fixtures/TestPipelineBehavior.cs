using Errand.Abstractions;
using Errand.Core;

namespace Errand.UnitTests.Fixtures;

public class TestPipelineBehavior1 : IPipelineBehavior<TestRequest, TestResponse>
{
    private readonly List<string>? _executionOrder;

    public TestPipelineBehavior1(List<string>? executionOrder = null)
    {
        _executionOrder = executionOrder;
    }

    public async Task<TestResponse> Handle(
        TestRequest request,
        RequestHandlerDelegate<TestResponse> next,
        CancellationToken cancellationToken)
    {
        _executionOrder?.Add("Behavior1-Before");
        var response = await next();
        _executionOrder?.Add("Behavior1-After");
        return response;
    }
}

public class TestPipelineBehavior2 : IPipelineBehavior<TestRequest, TestResponse>
{
    private readonly List<string>? _executionOrder;

    public TestPipelineBehavior2(List<string>? executionOrder = null)
    {
        _executionOrder = executionOrder;
    }

    public async Task<TestResponse> Handle(
        TestRequest request,
        RequestHandlerDelegate<TestResponse> next,
        CancellationToken cancellationToken)
    {
        _executionOrder?.Add("Behavior2-Before");
        var response = await next();
        _executionOrder?.Add("Behavior2-After");
        return response;
    }
}

public class TestOpenGenericBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        return await next();
    }
}