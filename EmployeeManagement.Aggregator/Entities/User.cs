namespace EmployeeManagement.Aggregator.Entities
{
    public class User
    {
        public int UserId { get; private set; }

        public string Name { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public string Password { get; private set; } = string.Empty;

        public string Role { get; private set; } = string.Empty;

        public Employee? Employee { get; private set; }

        private User(
            string name,
            string email,
            string password,
            string role)
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }

        public static User Create(
            string name,
            string email,
            string password,
            string role)
        {
            return new User(
                name,
                email,
                password,
                role);
        }
    }
}