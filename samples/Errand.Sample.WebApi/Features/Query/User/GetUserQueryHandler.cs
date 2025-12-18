using Errand.Abstractions;

namespace Errand.Sample.WebApi.Features.Query.User;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, List<string>>
{
    public Task<List<string>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        List<string> users = new()
        {
            "Alice",
            "Bob",
            "Charlie"
        };
        return Task.FromResult(users);
    }
}
