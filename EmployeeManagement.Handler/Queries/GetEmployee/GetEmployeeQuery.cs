namespace EmployeeManagement.Handler.Queries.GetEmployee
{
    public class GetEmployeeQuery{
        public int EmployeeId {get;}
        
        public GetEmployeeQuery(int employeeId)
        {
            EmployeeId = employeeId;
        }

    }
}