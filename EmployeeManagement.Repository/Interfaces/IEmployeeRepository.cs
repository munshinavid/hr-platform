

namespace EmployeeManagement.Repository.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByUserIdAsync(int userId);

        //Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null);

        IQueryable<Employee> GetQueryable();
    }
}
