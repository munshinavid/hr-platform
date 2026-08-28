namespace Authentication.Handler.Services
{
    public interface IPasswordHasher
    {
        /// <summary>Hashes a plain-text password using BCrypt.</summary>
        string Hash(string password);

        /// <summary>Verifies a plain-text password against its BCrypt hash.</summary>
        bool Verify(string password, string passwordHash);
    }
}
