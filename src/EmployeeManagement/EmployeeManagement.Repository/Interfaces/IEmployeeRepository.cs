using EmployeeManagement.Aggregator.Entities;

namespace EmployeeManagement.Repository.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<EmployeeAggregatorRoot>
    {
        Task<EmployeeAggregatorRoot?> GetByUserIdAsync(int userId);

        Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null);
        Task<(List<EmployeeAggregatorRoot> Employees, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize
        );

        IQueryable<EmployeeAggregatorRoot> GetQueryable();
    }
}
