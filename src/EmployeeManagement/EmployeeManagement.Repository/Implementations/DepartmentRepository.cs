using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Repository.Data;
using EmployeeManagement.Aggregator.Entities;

namespace EmployeeManagement.Repository.Implementations
{
    public class DepartmentRepository : GenericRepository<DepartmentAggregatorRoot>, IDepartmentRepository
    {
        public DepartmentRepository(EmployeeDbContext context) : base(context)
        {
        }
    }
}
