namespace IdentityManagement.DTO.Response
{
    /// <summary>
    /// Lightweight account-status projection.
    /// Contains only what an Orchestrator gatekeeper legitimately needs:
    /// whether the account exists and is currently active.
    /// </summary>
    public class UserStatusResponse
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
