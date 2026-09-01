using System.Collections.Generic;
using System.Threading.Tasks;
using LeaveManagement.Aggregator.Entities;

namespace LeaveManagement.Repository.Interfaces
{
    public interface ILeaveRequestRepository : IGenericRepository<LeaveRequest>
    {
        Task<(List<LeaveRequest> Requests, int TotalCount)> GetPagedAsync(
            int? employeeId,
            int? leaveTypeId,
            string? status,
            int pageNumber,
            int pageSize);
    }
}
