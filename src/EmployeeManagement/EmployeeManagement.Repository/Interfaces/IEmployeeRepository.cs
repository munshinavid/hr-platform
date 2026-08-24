using EmployeeManagement.Aggregator.Entities;

namespace EmployeeManagement.Repository.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByUserIdAsync(int userId);

        Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null);
        Task<(List<Employee> Employees, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize
        );

        IQueryable<Employee> GetQueryable();
    }
}
