using EmployeeManagement.Aggregator.Aggregates;

namespace EmployeeManagement.Repository.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<EmployeeAggregateRoot>
    {
        Task<EmployeeAggregateRoot?> GetByUserIdAsync(int userId);

        Task<bool> EmailExistsAsync(string email, int? excludeEmployeeId = null);
        Task<(List<EmployeeAggregateRoot> Employees, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize
        );

        IQueryable<EmployeeAggregateRoot> GetQueryable();
    }
}
