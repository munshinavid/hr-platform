using System.Threading.Tasks;
using LeaveManagement.Aggregator.Aggregates;
using LeaveManagement.Repository.Data;
using LeaveManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Repository.Implementations
{
    public class LeaveBalanceRepository : GenericRepository<LeaveBalanceAggregateRoot>, ILeaveBalanceRepository
    {
        public LeaveBalanceRepository(LeaveDbContext context) : base(context)
        {
        }

        public override async Task<LeaveBalanceAggregateRoot?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(b => b.LeaveType)
                .FirstOrDefaultAsync(b => b.LeaveBalanceId == id);
        }

        public async Task<LeaveBalanceAggregateRoot?> GetByEmployeeAndTypeAsync(int employeeId, int leaveTypeId, int year)
        {
            return await _dbSet
                .Include(b => b.LeaveType)
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == leaveTypeId && b.Year == year);
        }
    }
}
