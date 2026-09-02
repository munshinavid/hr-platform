namespace Orchestrator.DTO.Onboarding
{
    public class CreateEmployeeOnboardingResponse
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

