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
    }
}
