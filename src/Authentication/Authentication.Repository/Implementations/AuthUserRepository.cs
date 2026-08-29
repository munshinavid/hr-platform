using Authentication.Aggregator.Entities;
using Authentication.Repository.Data;
using Authentication.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Repository.Implementations
{
    public class AuthUserRepository : IAuthUserRepository
    {
        private readonly AuthDbContext _context;

        public AuthUserRepository(AuthDbContext context)
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
