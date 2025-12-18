using Errand.Abstractions;

namespace Errand.UnitTests.Fixtures;

public class TestRequestHandler : IRequestHandler<TestRequest, TestResponse>
{
    private readonly Action? _onHandle;

    public TestRequestHandler(Action? onHandle = null)
    {
        _onHandle = onHandle;
    }

    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        _onHandle?.Invoke();
        return Task.FromResult(new TestResponse { Result = request.Value });
    }
}

public class AlternativeTestRequestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestResponse { Result = request.Value * 2 });
    }
}

public class ThrowingTestRequestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Test exception");
    }
}

public abstract class AbstractTestHandler : IRequestHandler<TestRequest, TestResponse>
{
    public abstract Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken);
}