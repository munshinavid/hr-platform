namespace IdentityManagement.DTO.Command
{
    /// <summary>
    /// Suspends a user account so it can no longer authenticate.
    /// Used by Orchestrator during Employee Offboarding (Phase D).
    /// </summary>
    public class DeactivateUserCommand
    {
        public int UserId { get; set; }
    }
}
