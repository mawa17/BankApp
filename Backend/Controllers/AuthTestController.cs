#if DEBUG

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthTestController : ControllerBase
{
    [HttpGet]
    public IActionResult Everyone()
    {
        return Ok(new
        {
            Message = "Everyone can see this",
            User = User.Identity?.Name,
        });
    }

    [Authorize]
    [HttpGet]
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
    [HttpGet]
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

#endif