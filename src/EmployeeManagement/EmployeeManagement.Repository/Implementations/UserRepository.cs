using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Repository.Data;
using EmployeeManagement.Aggregator.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repository.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(EmployeeDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(
        string email,
        int? excludeUserId = null)
        {
            return await _dbSet.AnyAsync(u =>
                u.Email == email &&
                (!excludeUserId.HasValue ||
                 u.UserId != excludeUserId.Value));
        }
    }
}
