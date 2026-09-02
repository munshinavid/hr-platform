namespace LeaveManagement.DTO.Query
{
    public class GetLeaveBalanceQuery
    {
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int Year { get; set; }
    }
}
