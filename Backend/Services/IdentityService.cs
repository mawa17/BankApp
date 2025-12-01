using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public interface IIdentityService
{
    // Create
    Task<IdentityResult> CreateIdentityAsync(LoginModel model, IEnumerable<string>? roles = null);

    // Read
    Task<IdentityUserEx[]?> FindIdentitiesAsync(string query, int maxResults = int.MaxValue);
    Task<IdentityUserEx?> FindIdentityAsync(string query);

    // Update
    Task<IdentityResult> GrantRolesAsync(string query, IEnumerable<string> roles);
    Task<IdentityResult> RevokeRolesAsync(string query, IEnumerable<string> roles);

    // Delete
    Task<IdentityResult> DeleteIdentityAsync(string query);
}

public sealed class IdentityService(UserManager<IdentityUserEx> userManager, IDbService dbService) : IIdentityService
{
    // Create
    public async Task<IdentityResult> CreateIdentityAsync(LoginModel model, IEnumerable<string>? roles = null)
    {
        if (String.IsNullOrWhiteSpace(model.Username))
            return IdentityResult.Failed(new IdentityError { Description = "Username is required." });

        if (String.IsNullOrWhiteSpace(model.Password))
            return IdentityResult.Failed(new IdentityError { Description = "Password is required." });

        var user = new IdentityUserEx
        {
            UserName = model.Username,
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return result;

        if (roles != null && roles.Any())
        {
            var roleResult = await userManager.AddToRolesAsync(user, NormalizeRoles(roles));
            if (!roleResult.Succeeded)
                return roleResult;
        }

        await dbService.AddAsync(new AccountModel
        {
            UserId = user.Id,
        });

        return result;
    }

    // Read
    public async Task<IdentityUserEx[]?> FindIdentitiesAsync(string query, int maxResults = int.MaxValue)
    {
        if (String.IsNullOrWhiteSpace(query))
            return Array.Empty<IdentityUserEx>();

        var pattern = $"%{query}%";
        return await userManager.Users
            .Where(u =>
                EF.Functions.Like(u.UserName, pattern) ||
                EF.Functions.Like(u.Email, pattern))
            .Take(Math.Max(maxResults, 0)) 
            .ToArrayAsync();
    }
    public async Task<IdentityUserEx?> FindIdentityAsync(string query) => (await FindIdentitiesAsync(query, 1))?.SingleOrDefault();

    // Update
    public async Task<IdentityResult> GrantRolesAsync(string query, IEnumerable<string> roles)
    {
        if (roles == null || !roles.Any())
            return IdentityResult.Failed(new IdentityError { Description = "No roles provided." });

        var user = await FindIdentityAsync(query);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await userManager.AddToRolesAsync(user, NormalizeRoles(roles));
    }

    public async Task<IdentityResult> RevokeRolesAsync(string query, IEnumerable<string> roles)
    {
        if (roles == null || !roles.Any())
            return IdentityResult.Failed(new IdentityError { Description = "No roles provided." });

        var user = await FindIdentityAsync(query);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await userManager.RemoveFromRolesAsync(user, NormalizeRoles(roles));
    }

    // Delete
    public async Task<IdentityResult> DeleteIdentityAsync(string query)
    {
        var user = await FindIdentityAsync(query);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        
        return await userManager.DeleteAsync(user);
    }

    // Helper: normalize role names to all uppercase
    private IEnumerable<string> NormalizeRoles(IEnumerable<string> roles) =>
        roles.Select(r => r.Trim().ToUpperInvariant());
}