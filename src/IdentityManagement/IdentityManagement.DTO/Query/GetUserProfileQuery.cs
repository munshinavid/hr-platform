namespace IdentityManagement.DTO.Query
{
    /// <summary>
    /// Returns the full identity profile for a user.
    /// Used by Orchestrator for composite read aggregation (Employee Dashboard).
    /// Never returns PasswordHash or any credential secret.
    /// </summary>
    public class GetUserProfileQuery
    {
        public int UserId { get; set; }
    }
}
