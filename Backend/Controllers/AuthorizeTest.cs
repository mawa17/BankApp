using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizeTest : ControllerBase
{
    [Authorize]
    [HttpGet("secret")]
    public IActionResult Secret()
    {
        return Ok("✅ You are authorized and can see this message!");
    }
}