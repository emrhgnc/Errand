# Getting Started with Errand

This guide will help you get started with Errand in your . NET application.

## Installation

Install the Errand package from NuGet:

```bash
dotnet add package Errand
```

## Basic Setup

### 1. Define Your Request and Response

```csharp
using Errand. Abstractions;

// Request with response
public record GetUserQuery(int UserId) : IRequest<UserDto>;

// Response DTO
public record UserDto(int Id, string Name, string Email);
```

### 2. Create a Handler

```csharp
using Errand.Abstractions;

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
        
        if (user == null)
            throw new NotFoundException($"User {request.UserId} not found");

        return new UserDto(user.Id, user.Name, user.Email);
    }
}
```

### 3. Register Errand in DI Container

```csharp
// Program.cs
using Errand.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Errand and scan for handlers in the specified assembly
builder.Services.AddErrand(typeof(Program).Assembly);

var app = builder.Build();
```

### 4. Use in Your Controller or Endpoint

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

## Request Types

### Query (Read Operation)

```csharp
// Returns data
public record GetProductsQuery(int PageNumber, int PageSize) : IRequest<List<ProductDto>>;
```

### Command with Response

```csharp
// Modifies data and returns a value (e.g., ID of created entity)
public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;
```

### Command without Response

```csharp
// Modifies data but doesn't return a value
public record DeleteProductCommand(int ProductId) : IRequest;

// Handler
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // Delete logic... 
    }
}
```

