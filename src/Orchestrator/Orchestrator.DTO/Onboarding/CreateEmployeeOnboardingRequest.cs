namespace Orchestrator.DTO.Onboarding
{
    /// <summary>
    /// Client-facing request for the employee onboarding workflow.
    ///
    /// This is the Orchestrator's public contract. It is intentionally
    /// flat — the Orchestrator Handler will split this into subsystem-specific
    /// commands (RegisterUserCommand for Identity, CreateEmployeeCommand for
    /// EmployeeManagement) via IServiceBus.
    /// </summary>
    public class CreateEmployeeOnboardingRequest
    {
        // ── Identity-side fields ──────────────────────────────────────────────
        /// <summary>Full name used for the Identity user account.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Email used for Identity login AND stored as Employee.Email.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Plain-text password — hashed inside IdentityManagement.Handler.</summary>
        public string Password { get; set; } = string.Empty;

        // ── Employee-side fields ──────────────────────────────────────────────
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

