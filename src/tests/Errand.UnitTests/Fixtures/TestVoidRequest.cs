using Errand.Abstractions;
using Errand.Core;

namespace Errand.UnitTests.Fixtures;

public class TestVoidRequest : IRequest<Unit>
{
}

public class TestVoidRequestHandler : IRequestHandler<TestVoidRequest, Unit>
{
    public static bool WasCalled { get; private set; }

    public Task<Unit> Handle(TestVoidRequest request, CancellationToken cancellationToken)
    {
        WasCalled = true;
        return Task.FromResult(Unit.Value);
    }

    public static void Reset()
    {
        WasCalled = false;
    }
}