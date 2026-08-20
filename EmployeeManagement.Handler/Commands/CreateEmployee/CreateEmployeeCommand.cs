using EmployeeManagement.DTO.Employee;

namespace EmployeeManagement.Handler.Commands.CreateEmployee
{
    public class CreateEmployeeCommand
    {
        public CreateEmployeeRequest Request { get; }

        public CreateEmployeeCommand(CreateEmployeeRequest request)
        {
            Request = request;
        }
    }
}
