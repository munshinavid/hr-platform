using LeaveManagement.Aggregator.Aggregates;
using LeaveManagement.Repository.Data;
using LeaveManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Repository.Implementations
{
    public class LeaveRequestRepository : GenericRepository<LeaveRequestAggregateRoot>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(LeaveDbContext context) : base(context)
        {
        }

        public override async Task<LeaveRequestAggregateRoot?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(r => r.LeaveType)
                .FirstOrDefaultAsync(r => r.LeaveRequestId == id);
        }

        public async Task<(List<LeaveRequestAggregateRoot> Requests, int TotalCount)> GetPagedAsync(
            int? employeeId,
            int? leaveTypeId,
            string? status,
            int pageNumber,
            int pageSize)
        {
            var query = _dbSet.Include(r => r.LeaveType).AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(r => r.EmployeeId == employeeId.Value);

            if (leaveTypeId.HasValue)
                query = query.Where(r => r.LeaveTypeId == leaveTypeId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(r => r.Status == status);

            var totalCount = await query.CountAsync();

            var requests = await query
                .OrderByDescending(r => r.RequestedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (requests, totalCount);
        }
    }
}
