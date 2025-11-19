namespace Frontend.Services
{
    public class AuthService
    {
        public bool IsLoggedIn { get; set; } = false;
        public string UserName { get; set; } = "";

        private List<User> users = new List<User>();
        public bool Register(string firstName, string lastName, string userName, string email, string password)
        {
            if (users.Any(u=>u.UserName==userName))
                return false;

            users.Add(new User
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = userName,
                Email = email,
                Password = password
            });
            return true;
            
        }

        public bool Login(string userName, string password)
        {
            var user = users.FirstOrDefault(u => u.UserName == userName && u.Password == password);
            if (user != null)
            {
                IsLoggedIn = true;
                UserName = userName;
                return true;
            }

            return false;
        }

        public class User
        {
            public string FirstName {get; set;}
            public string LastName { get; set;}
            public string UserName { get; set;}
            public string Email { get; set;}
            public string Password { get; set;}

        }
    }
}
