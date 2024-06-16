using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PdmoonblogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("getSession")]
    [Authorize]
    public IActionResult GetSession()
    {
        return Ok(User.Identity?.Name);
    }
}
