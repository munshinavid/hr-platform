namespace EmployeeManagement.Aggregator.Aggregates
{
    public class DepartmentAggregateRoot
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;
    }
}