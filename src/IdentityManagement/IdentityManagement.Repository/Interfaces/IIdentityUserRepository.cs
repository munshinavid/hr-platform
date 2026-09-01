using IdentityManagement.Aggregator.Entities;

namespace IdentityManagement.Repository.Interfaces
{
    public interface IIdentityUserRepository
    {
        Task<UserAggregatorRoot?> GetByIdAsync(int userId);
        Task<UserAggregatorRoot?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> AddAsync(UserAggregatorRoot user);
        Task<bool> UpdateAsync(UserAggregatorRoot user);
        Task<bool> DeleteAsync(int userId);
    }
}



