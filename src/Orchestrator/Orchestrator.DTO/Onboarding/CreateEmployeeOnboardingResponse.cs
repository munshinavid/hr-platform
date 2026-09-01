namespace Orchestrator.DTO.Onboarding
{
    /// <summary>
    /// Orchestrator-specific success response for the employee onboarding workflow.
    /// Carries the key identifiers from both subsystems so the client knows
    /// which UserId and EmployeeId were created.
    /// </summary>
    public class CreateEmployeeOnboardingResponse
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

