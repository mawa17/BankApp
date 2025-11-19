using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.Services;

public interface IUserService
{
    // Create
    Task<IdentityResult> CreateUserAsync(LoginModel model, IEnumerable<string>? roles = null);

    // Read
    Task<IdentityUserEx[]> FindUsersAsync(string query, int maxResults = int.MaxValue);
    Task<IdentityUserEx?> FindUserAsync(string query);

    // Update
    Task<IdentityResult> GrantRolesAsync(string query, IEnumerable<string> roles);
    Task<IdentityResult> RevokeRolesAsync(string query, IEnumerable<string> roles);

    // Delete
    Task<IdentityResult> DeleteUserAsync(string query);
}

public sealed class UserService(UserManager<IdentityUserEx> userManager) : IUserService
{
    private readonly UserManager<IdentityUserEx> _userManager = userManager;

    // Create
    public async Task<IdentityResult> CreateUserAsync(LoginModel model, IEnumerable<string>? roles = null)
    {
        if (String.IsNullOrWhiteSpace(model.Username))
            return IdentityResult.Failed(new IdentityError { Description = "Username is required." });

        if (String.IsNullOrWhiteSpace(model.Password))
            return IdentityResult.Failed(new IdentityError { Description = "Password is required." });

        var user = new IdentityUserEx
        {
            UserName = model.Username,
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded) return result;

        if (roles != null && roles.Any())
            await _userManager.AddToRolesAsync(user, NormalizeRoles(roles));

        return result;
    }

    // Read
    public async Task<IdentityUserEx[]> FindUsersAsync(string query, int maxResults = int.MaxValue)
    {
        if (String.IsNullOrWhiteSpace(query))
            return Array.Empty<IdentityUserEx>();

        var pattern = $"%{query}%";
        return await _userManager.Users
            .Where(u =>
                EF.Functions.Like(u.UserName, pattern) ||
                EF.Functions.Like(u.Email, pattern) ||
                EF.Functions.Like(u.Id, pattern))
            .Take(Math.Max(maxResults, 0))
            .ToArrayAsync();
    }
    public async Task<IdentityUserEx?> FindUserAsync(string query) => (await FindUsersAsync(query, 1)).SingleOrDefault();

    // Update
    public async Task<IdentityResult> GrantRolesAsync(string query, IEnumerable<string> roles)
    {
        if (roles == null || !roles.Any())
            return IdentityResult.Failed(new IdentityError { Description = "No roles provided." });

        var user = await FindUserAsync(query);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userManager.AddToRolesAsync(user, NormalizeRoles(roles));
    }

    public async Task<IdentityResult> RevokeRolesAsync(string query, IEnumerable<string> roles)
    {
        if (roles == null || !roles.Any())
            return IdentityResult.Failed(new IdentityError { Description = "No roles provided." });

        var user = await FindUserAsync(query);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userManager.RemoveFromRolesAsync(user, NormalizeRoles(roles));
    }

    // Delete
    public async Task<IdentityResult> DeleteUserAsync(string query)
    {
        var user = await FindUserAsync(query);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userManager.DeleteAsync(user);
    }

    // Helper: normalize role names to all uppercase
    private IEnumerable<string> NormalizeRoles(IEnumerable<string> roles) =>
        roles.Select(r => r.Trim().ToUpperInvariant());
}