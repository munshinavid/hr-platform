using IdentityManagement.Aggregator.Exceptions;
using IdentityManagement.Aggregator.Mapping;
using IdentityManagement.DTO.Command;

namespace IdentityManagement.Aggregator.Entities
{
    public class UserAggregatorRoot
    {
        public int UserId { get; set; }

        // Identity credential — the login email.
        // EmployeeManagement owns the HR display name and work email separately.
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        // Authorization claim embedded in JWT.
        public string Role { get; set; } = string.Empty;

        // Account lifecycle — determines whether the user can authenticate.
        // This is NOT the same as Employee.Status (employment state).
        // IsActive = false means the account is suspended/deactivated.
        // Default: true for all newly registered users.
        public bool IsActive { get; set; } = true;

        // Audit metadata.
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // ── Domain factory ──────────────────────────────────────────────

        public static UserAggregatorRoot MapToAggregator(
            RegisterUserCommand command,
            string passwordHash,
            string role)
        {
            return UserMapper.MapToAggregator(command, passwordHash, role);
        }

        // ── Domain behaviour ────────────────────────────────────────────

        /// <summary>
        /// Deactivates the account so the user can no longer authenticate.
        /// Idempotent: deactivating an already-inactive account is a no-op
        /// that returns false so the caller can surface a meaningful message.
        /// </summary>
        public bool Deactivate()
        {
            if (!IsActive)
                return false;   // already inactive — caller decides how to respond

            IsActive  = false;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }

        /// <summary>
        /// Re-activates a previously deactivated account.
        /// Idempotent: activating an already-active account returns false.
        /// </summary>
        public bool Activate()
        {
            if (IsActive)
                return false;   // already active — caller decides how to respond

            IsActive  = true;
            UpdatedAt = DateTime.UtcNow;
            return true;
        }
    }
}
