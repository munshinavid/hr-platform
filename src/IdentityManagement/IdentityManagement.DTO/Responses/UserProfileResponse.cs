namespace IdentityManagement.DTO.Response
{
    /// <summary>
    /// Full identity profile response.
    /// Used by Orchestrator for composite reads (e.g., Employee Dashboard dashboard).
    ///
    /// Security invariants:
    ///   - PasswordHash is NEVER present.
    ///   - No Employee entity or navigation properties.
    ///   - Name is intentionally absent — owned by EmployeeManagement.
    /// </summary>
    public class UserProfileResponse
    {
        public int UserId { get; set; }

        /// <summary>Login / identity email.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Authorization role embedded in JWT claims.</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>Whether the account can currently authenticate.</summary>
        public bool IsActive { get; set; }

        /// <summary>UTC timestamp when this account was created.</summary>
        public DateTime CreatedAt { get; set; }
    }
}
