namespace IdentityManagement.DTO.Query
{
    /// <summary>
    /// Returns the full identity profile for a user.
    /// Used by Orchestrator for composite read aggregation (Employee 360).
    /// Never returns PasswordHash or any credential secret.
    /// </summary>
    public class GetUserProfileQuery
    {
        public int UserId { get; set; }
    }
}
