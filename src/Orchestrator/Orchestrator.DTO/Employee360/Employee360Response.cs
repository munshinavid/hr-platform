using System.Collections.Generic;
using EmployeeManagement.DTO.Response;
using IdentityManagement.DTO.Response;
using LeaveManagement.DTO.Response;

namespace Orchestrator.DTO.Employee360
{
    public class Employee360Response
    {
        public EmployeeResponse? Employee { get; set; }
        public UserProfileResponse? Identity { get; set; }
        public IEnumerable<LeaveBalanceResponse>? LeaveBalances { get; set; }
    }
}
