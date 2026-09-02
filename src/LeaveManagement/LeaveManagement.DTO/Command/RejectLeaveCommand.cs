namespace LeaveManagement.DTO.Command
{
    public class RejectLeaveCommand
    {
        public int LeaveRequestId { get; set; }
        public int RejectedByEmployeeId { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
    }
}
