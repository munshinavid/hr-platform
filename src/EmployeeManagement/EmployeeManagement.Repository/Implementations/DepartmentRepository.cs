using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Repository.Data;
using EmployeeManagement.Aggregator.Aggregates;

namespace EmployeeManagement.Repository.Implementations
{
    public class DepartmentRepository : GenericRepository<DepartmentAggregateRoot>, IDepartmentRepository
    {
        public DepartmentRepository(EmployeeDbContext context) : base(context)
        {
        }
    }
}
