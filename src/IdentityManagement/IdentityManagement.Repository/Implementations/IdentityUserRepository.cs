using IdentityManagement.Aggregator.Entities;
using IdentityManagement.Repository.Data;
using IdentityManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityManagement.Repository.Implementations
{
    public class IdentityUserRepository : IIdentityUserRepository
    {
        private readonly IdentityDbContext _context;

        public IdentityUserRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<UserAggregatorRoot?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> AddAsync(UserAggregatorRoot user)
        {
            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

