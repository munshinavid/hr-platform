namespace EmployeeManagement.DTO.Employee
{
    public class GetEmployeesRequest
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}