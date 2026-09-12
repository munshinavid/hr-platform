using System.Collections.Generic;
using System.Threading.Tasks;
using LeaveManagement.Aggregator.Aggregates;

namespace LeaveManagement.Repository.Interfaces
{
    public interface ILeaveRequestRepository : IGenericRepository<LeaveRequestAggregateRoot>
    {
        Task<(List<LeaveRequestAggregateRoot> Requests, int TotalCount)> GetPagedAsync(
            int? employeeId,
            int? leaveTypeId,
            string? status,
            int pageNumber,
            int pageSize);
    }
}
