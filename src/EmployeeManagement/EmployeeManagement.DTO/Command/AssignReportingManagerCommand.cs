namespace EmployeeManagement.DTO.Command
{
    public class AssignReportingManagerCommand
    {
        public int EmployeeId { get; set; }
        public int ReportingManagerId { get; set; }
    }
}
