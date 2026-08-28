using EmployeeManagement.Aggregator.Entities;

namespace EmployeeManagement.Repository.Interfaces
{
    public interface IUserRepository : IGenericRepository<UserAggregatorRoot>
    {
        Task<UserAggregatorRoot?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    }
}
