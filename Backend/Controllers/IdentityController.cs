using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
[Authorize(Roles = "ADMIN")]
public sealed class IdentityController(IIdentityService userService) : ControllerBase
{
    private readonly IIdentityService _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LoginModel model, [FromQuery(Name = "roles")] string[]? roles = null)
    {
        var result = await _userService.CreateIdentityAsync(model, roles);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "User created successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery(Name = "value")] string query, [FromQuery(Name = "max")] int maxResults = int.MaxValue)
    {
        var users = await _userService.FindIdentitiesAsync(query, maxResults);
        return Ok(users);
    }

    [HttpGet("{query}")]
    public async Task<IActionResult> Get([FromRoute] string query)
    {
        var user = await _userService.FindIdentityAsync(query);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }

    [HttpPost("{query}")]
    public async Task<IActionResult> GrantRoles([FromRoute] string query, [FromBody] string[] roles)
    {
        var result = await _userService.GrantRolesAsync(query, roles);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Roles granted successfully" });
    }

    [HttpDelete("{query}")]
    public async Task<IActionResult> RevokeRoles(string query, [FromBody] string[] roles)
    {
        var result = await _userService.RevokeRolesAsync(query, roles);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Roles revoked successfully" });
    }

    [HttpDelete("{query}")]
    public async Task<IActionResult> Delete(string query)
    {
        var result = await _userService.DeleteIdentityAsync(query);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "User deleted successfully" });
    }
}