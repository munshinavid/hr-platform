using System.Threading.Tasks;
using LeaveManagement.Aggregator.Entities;

namespace LeaveManagement.Repository.Interfaces
{
    public interface ILeaveBalanceRepository : IGenericRepository<LeaveBalance>
    {
        Task<LeaveBalance?> GetByEmployeeAndTypeAsync(int employeeId, int leaveTypeId, int year);
    }
}
