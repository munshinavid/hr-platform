using System.Threading.Tasks;
using LeaveManagement.Aggregator.Aggregates;

namespace LeaveManagement.Repository.Interfaces
{
    public interface ILeaveBalanceRepository : IGenericRepository<LeaveBalanceAggregateRoot>
    {
        Task<LeaveBalanceAggregateRoot?> GetByEmployeeAndTypeAsync(int employeeId, int leaveTypeId, int year);
    }
}
