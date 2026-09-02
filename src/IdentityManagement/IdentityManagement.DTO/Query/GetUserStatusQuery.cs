namespace IdentityManagement.DTO.Query
{
    /// <summary>
    /// Returns only the account lifecycle status for a user.
    /// Lightweight query used by Orchestrator as a gatekeeper before
    /// cross-context operations (e.g., Leave Application).
    /// </summary>
    public class GetUserStatusQuery
    {
        public int UserId { get; set; }
    }
}
