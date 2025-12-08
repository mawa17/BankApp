public interface IUserService
{
    Task<bool> UserExistsAsync(string username);
}