namespace Authentication.Aggregator.Entities
{
    /// <summary>
    /// Represents an authenticated user identity in the Authentication bounded context.
    /// This is the Authentication domain model — independent of EmployeeManagement entities.
    /// </summary>
    public class AuthUser
    {
        public int UserId { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Role { get; private set; } = string.Empty;

        private AuthUser() { }

        public static AuthUser Create(int userId, string email, string passwordHash, string role)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

            return new AuthUser
            {
                UserId = userId,
                Email = email,
                PasswordHash = passwordHash,
                Role = role
            };
        }
    }
}
