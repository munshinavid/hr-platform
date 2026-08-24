namespace Authentication.DTO.Commands
{
    /// <summary>
    /// Request contract for the Login use case.
    /// JWT generation is the responsibility of Authentication.Handler / Authentication subsystem.
    /// </summary>
    public class LoginCommand
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
