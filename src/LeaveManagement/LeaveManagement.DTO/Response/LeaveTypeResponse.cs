namespace LeaveManagement.DTO.Response
{
    public class LeaveTypeResponse
    {
        public int LeaveTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DefaultDaysPerYear { get; set; }
        public bool IsActive { get; set; }
    }
}
