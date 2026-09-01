namespace Orchestrator.DTO.Offboarding
{
    public class OffboardEmployeeResponse
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public int LeavesCancelledCount { get; set; }
        public bool LeaveCleanupFailed { get; set; }
    }
}
