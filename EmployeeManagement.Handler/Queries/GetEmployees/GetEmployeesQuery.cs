namespace EmployeeManagement.Handler.Queries.GetEmployees
{
    public class GetEmployeesQuery
    {
        public int PageNumber { get; }

        public int PageSize { get; }

        public GetEmployeesQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
