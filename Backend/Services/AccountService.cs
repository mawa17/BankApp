using Backend.Models;
using System.Security.Principal;

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
    private async Task<AccountModel?> GetByAsync(string query)
    {
        var identity = await identityService.FindIdentityAsync(query);
        if (identity is null) return null;
        return dbService.Query<AccountModel>().FirstOrDefault(x => x.UserId == identity.Id);
    }
}