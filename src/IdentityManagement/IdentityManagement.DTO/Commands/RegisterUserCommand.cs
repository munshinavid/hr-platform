namespace IdentityManagement.DTO.Command
{
    /// <summary>
    /// Creates a new user credential in IdentityManagement.
    /// Name is intentionally absent — the HR profile name is owned by EmployeeManagement.
    /// </summary>
    public class RegisterUserCommand
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

