namespace Frontend.Services
{
    public class AuthService
    {
        public bool IsLoggedIn { get; set; } = false;
        public string UserName { get; set; } = "";
        public bool IsAdmin { get; private set; } = false;

        private List<User> users = new List<User>();

        // Fast admin-bruger hardcoded
        private readonly User adminUser = new User
        {
            FirstName = "Admin",
            LastName = "User",
            UserName = "admin",
            Email = "admin@bank.com",
            Password = "admin123",
            IsAdmin = true
        };

        public AuthService()
        {
            // Tilføj admin-brugeren som “dummy” på listen
            users.Add(adminUser);
        }

        public bool Register(string firstName, string lastName, string userName, string email, string password)
        {
            if (users.Any(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)))
                return false;

            users.Add(new User
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = password,
                IsAdmin = false
            });

            return true;
        }

        public bool Login(string userName, string password)
        {
            var user = users.FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                                              && u.Password == password);
            if (user != null)
            {
                IsLoggedIn = true;
                UserName = userName;
                IsAdmin = user.IsAdmin;
                return true;
            }

            return false;
        }

        public void Logout()
        {
            IsLoggedIn = false;
            UserName = "";
            IsAdmin = false;
        }

        public class User
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public bool IsAdmin { get; set; } = false;
        }
    }
}
