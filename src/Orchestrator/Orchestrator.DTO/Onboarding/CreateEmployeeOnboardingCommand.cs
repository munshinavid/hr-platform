namespace Orchestrator.DTO.Onboarding
{
    public class CreateEmployeeOnboardingCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
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
