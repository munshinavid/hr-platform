using EmployeeManagement.Aggregator.Entities;

namespace EmployeeManagement.Repository.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
