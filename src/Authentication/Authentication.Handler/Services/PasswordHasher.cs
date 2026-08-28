namespace Authentication.Handler.Services
{
    /// <summary>
    /// BCrypt password hasher.
    /// Uses the same BCrypt.Net-Next library already used in
    /// EmployeeManagement.Handler (CreateEmployeeHandler) for consistency.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
