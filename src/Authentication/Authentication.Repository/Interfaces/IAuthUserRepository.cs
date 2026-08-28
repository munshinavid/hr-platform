using Authentication.Aggregator.Entities;

namespace Authentication.Repository.Interfaces
{
    public interface IAuthUserRepository
    {
        Task<UserAggregatorRoot?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> AddAsync(UserAggregatorRoot user);
    }
}

