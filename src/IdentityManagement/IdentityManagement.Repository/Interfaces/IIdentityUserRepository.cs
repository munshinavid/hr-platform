using IdentityManagement.Aggregator.Aggregates;

namespace IdentityManagement.Repository.Interfaces
{
    public interface IIdentityUserRepository
    {
        Task<UserAggregateRoot?> GetByIdAsync(int userId);
        Task<UserAggregateRoot?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> AddAsync(UserAggregateRoot user);
        Task<bool> UpdateAsync(UserAggregateRoot user);
        Task<bool> DeleteAsync(int userId);
    }
}



