using EmployeeManagement.Aggregator.Aggregates;

namespace EmployeeManagement.Repository.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<DepartmentAggregateRoot>
    {
    }
}
