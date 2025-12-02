using Backend.Models;

namespace Backend.Services;

public interface IAccountService
{
    /*
    ✅  Indsæt penge 
    ✅  Hæv penge
    ✅  Overfør penge
    Overblik over alle transaktioner unikt til AccountModel
    Gamble

    Rente system? med lån?

    Admin 
    Se alles transaktioner CRUD
    Stjæle penge?
    Ændre på settings?  Rente system? med lån?
    */
}

public class AccountService(IDbService dbService, IIdentityService identityService) : IAccountService
{
    public async Task<bool> DepositAsync(string name, decimal amount)
    {
        var account = await GetByAsync(name);
        if (account == null) return false;

        account.Balance += amount;
        
        return await dbService.SaveAsync();
    }

    public async Task<bool> WithdrawAsync(string name, decimal amount)
    {
        var account = await GetByAsync(name);
        if (account == null) return false;

        account.Balance = (account.Balance - amount) >= 0 ? account.Balance - amount : account.Balance;
        return await dbService.SaveAsync();
    }

    public async Task<bool> TransferAsync(string name1, string name2, decimal amount)
    {
        var account1 = await GetByAsync(name1);
        var account2 = await GetByAsync(name2);
        if ((account1 == null) || (account2 == null)) return false;

        if(!await WithdrawAsync(name1, amount)) return false;
        await DepositAsync(name2, amount);
        
        return await dbService.SaveAsync();
    }

    public async Task<bool> GambleAsync(string name)
    {
        var account = await GetByAsync(name);
        if (account == null) return false;

        return false;
    }

    private async Task<AccountModel?> GetByAsync(string query)
    {
        var identity = await identityService.FindIdentityAsync(query);
        if (identity is null) return null;
        return dbService.Query<AccountModel>().FirstOrDefault(x => x.UserId == identity.Id);
    }
}
