namespace EmployeeManagement.Aggregator.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "Employee";

        // Navigation
        public Employee? Employee { get; set; }


        public User(string name,
            string email,
            string password,
            string role)
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }
    }
}
