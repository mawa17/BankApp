public class MockUserService : IUserService
{
    private readonly List<string> _users = new List<string>
    {
        "Rasmus",
        "John",
        "Bente"
    };

    public Task<bool> UserExistsAsync(string username)
    {
        // simuler Api delay
        return Task.FromResult(_users.Contains(username));
    }
}