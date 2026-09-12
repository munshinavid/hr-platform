using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Repository.Data;
using EmployeeManagement.Aggregator.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repository.Implementations
{
    public class EmployeeRepository : GenericRepository<EmployeeAggregateRoot>, IEmployeeRepository
    {
        public EmployeeRepository(EmployeeDbContext context) : base(context)
        {
        }

        public override async Task<List<EmployeeAggregateRoot>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.Department)
                .ToListAsync();
        }

        public override async Task<EmployeeAggregateRoot?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<EmployeeAggregateRoot?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<bool> EmailExistsAsync(
            string email,
            int? excludeEmployeeId = null)
        {
            return await _dbSet
                .AnyAsync(e =>
                    e.Email == email &&
                    (!excludeEmployeeId.HasValue ||
                     e.EmployeeId != excludeEmployeeId.Value));
        }


        public IQueryable<EmployeeAggregateRoot> GetQueryable()
        {
            return _dbSet
                .Include(e => e.Department)
                .AsQueryable();
        }

        public async Task<(List<EmployeeAggregateRoot> Employees, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            var query = _dbSet
                .Include(e => e.Department)
                .OrderBy(e => e.EmployeeId);

            var totalCount = await query.CountAsync();

            var employees = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (employees, totalCount);
        }
    }
}
