namespace IdentityManagement.DTO.Command
{
    /// <summary>
    /// Re-activates a previously deactivated user account.
    /// Used by Orchestrator during compensation / re-hire workflows.
    /// </summary>
    public class ActivateUserCommand
    {
        public int UserId { get; set; }
    }
}
