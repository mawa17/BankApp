using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizeTest : ControllerBase
{
    [HttpGet("everyone")]
    public IActionResult Everyone()
    {
        return Ok(new
        {
            Message = "Everyone can see this",
            User = User.Identity?.Name,
        });
    }

    [Authorize]
    [HttpGet("loginOnly")]
    public IActionResult LoginOnly()
    {
        return Ok(new
        {
            Message = "Login required",
            UserName = User.Identity?.Name,
            IsAuthenticated = User.Identity?.IsAuthenticated,
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("adminOnly")]
    public IActionResult AdminOnly()
    {
        return Ok(new
        {
            Message = "Admin access only",
            UserName = User.Identity?.Name,
            IsAuthenticated = User.Identity?.IsAuthenticated,
            Roles = User.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList(),
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}
