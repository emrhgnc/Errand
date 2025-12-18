## 🚀 Quick Start

### 1. Define a Request and Handler

```csharp
using Errand.Abstractions;

// Request with response
public record GetUserQuery(int UserId) : IRequest<UserDto>;

// Response DTO
public record UserDto(int Id, string Name, string Email);

// Handler
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IUserRepository _repository;

    public GetUserQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId, cancellationToken);
        return new UserDto(user.Id, user.Name, user.Email);
    }
}
```

### 2. Register Errand in DI

```csharp
// Program.cs
builder.Services.AddErrand(typeof(Program).Assembly);
```

### 3. Use in Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IErrand _errand;

    public UsersController(IErrand errand)
    {
        _errand = errand;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var query = new GetUserQuery(id);
        var result = await _errand.Send(query);
        return Ok(result);
    }
}
```