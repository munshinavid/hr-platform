using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Repository.Data;
using EmployeeManagement.Aggregator.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repository.Implementations
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(EmployeeDbContext context) : base(context)
        {
        }

        public override async Task<List<Employee>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.Department)
                .Include(e => e.User)
                .ToListAsync();
        }

        public override async Task<Employee?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Department)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<Employee?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(e => e.Department)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        //public async Task<bool> EmailExistsAsync(
        //    string email,
        //    int? excludeEmployeeId = null)
        //{
        //    return await _dbSet
        //        .Include(e => e.User)
        //        .AnyAsync(e =>
        //            e.User.Email == email &&
        //            (!excludeEmployeeId.HasValue ||
        //             e.EmployeeId != excludeEmployeeId.Value));
        //}

        public IQueryable<Employee> GetQueryable()
        {
            return _dbSet
                .Include(e => e.Department)
                .Include(e => e.User)
                .AsQueryable();
        }

        public async Task<(List<Employee> Employees, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize)
        {
            var query = _dbSet
                .Include(e => e.Department)
                .Include(e => e.User)
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
