using Errand.Abstractions;

namespace Errand.UnitTests.Fixtures;

public class TestRequest : IRequest<TestResponse>
{
    public int Value { get; set; }
}

public class TestResponse
{
    public int Result { get; set; }
}