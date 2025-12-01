using Backend.Models;

namespace Backend.Services;

public interface IAccountService
{
    /*
    Indsæt penge
    Hæv penge
    Overfør penge
    Overblik over alle transaktioner unikt til AccountModel

    Rente system? med lån?

    Admin 
    Se alles transaktioner CRUD
    Stjæle penge?
    Ændre på settings?  Rente system? med lån?
    */
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