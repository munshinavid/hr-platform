namespace LeaveManagement.DTO.Command
{
    public class ApproveLeaveCommand
    {
        public int LeaveRequestId { get; set; }
        public int ApprovedByEmployeeId { get; set; }
    }
}
