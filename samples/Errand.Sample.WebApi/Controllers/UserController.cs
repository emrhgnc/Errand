using Errand.Abstractions;
using Errand.Sample.WebApi.Features.Query.User;
using Microsoft.AspNetCore.Mvc;

namespace Errand.Sample.WebApi.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController:ControllerBase
{
    private readonly IErrand _errand;

    public UserController(IErrand errand)
    {
        _errand = errand;
    }
    [HttpGet]
    public ActionResult<List<string>> GetUsers() { 
        var res = _errand.Send(new GetUserQuery());
        return Ok(res.Result);
    }
}
