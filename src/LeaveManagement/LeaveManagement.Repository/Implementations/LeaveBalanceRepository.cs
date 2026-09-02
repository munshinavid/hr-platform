using System.Threading.Tasks;
using LeaveManagement.Aggregator.Entities;
using LeaveManagement.Repository.Data;
using LeaveManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Repository.Implementations
{
    public class LeaveBalanceRepository : GenericRepository<LeaveBalance>, ILeaveBalanceRepository
    {
        public LeaveBalanceRepository(LeaveDbContext context) : base(context)
        {
        }

        public override async Task<LeaveBalance?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(b => b.LeaveType)
                .FirstOrDefaultAsync(b => b.LeaveBalanceId == id);
        }

        public async Task<LeaveBalance?> GetByEmployeeAndTypeAsync(int employeeId, int leaveTypeId, int year)
        {
            return await _dbSet
                .Include(b => b.LeaveType)
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == leaveTypeId && b.Year == year);
        }
    }
}
