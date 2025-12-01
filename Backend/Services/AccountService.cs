using Backend.Models;

namespace Backend.Services;

public interface IAccountService
{

}

public class AccountService(IDbService dbService, IIdentityService identityService) : IAccountService
{
    async Task ForeginKeyLookupExample()
    {
        var identity = await identityService.FindIdentityAsync("bob");
        if (identity is null) return;
        var account = dbService.Query<AccountModel>().FirstOrDefault(x => x.UserId == identity.Id);
    }
}