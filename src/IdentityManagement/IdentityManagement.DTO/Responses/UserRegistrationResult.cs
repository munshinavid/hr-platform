namespace IdentityManagement.DTO.Response
{
    /// <summary>
    /// Returned by the orchestration-facing RegisterUserCommand handler.
    /// Carries the newly created UserId so that downstream workflow steps
    /// (e.g. CreateEmployeeOnboardingHandler) can link the Identity user to
    /// an Employee record without accessing the Identity repository directly.
    /// </summary>
    public class UserRegistrationResult
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}

