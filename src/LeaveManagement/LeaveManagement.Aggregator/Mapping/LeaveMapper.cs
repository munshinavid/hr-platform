using LeaveManagement.Aggregator.Entities;
using LeaveManagement.DTO.Response;

namespace LeaveManagement.Aggregator.Mapping
{
    public static class LeaveMapper
    {
        public static LeaveBalanceResponse MapToResponse(LeaveBalance balance)
        {
            return new LeaveBalanceResponse
            {
                LeaveBalanceId = balance.LeaveBalanceId,
                EmployeeId = balance.EmployeeId,
                LeaveTypeId = balance.LeaveTypeId,
                LeaveTypeName = balance.LeaveType?.Name ?? string.Empty,
                Year = balance.Year,
                TotalDays = balance.TotalDays,
                UsedDays = balance.UsedDays,
                HeldDays = balance.HeldDays,
                AvailableDays = balance.AvailableDays,
                CreatedAt = balance.CreatedAt,
                UpdatedAt = balance.UpdatedAt
            };
        }

        public static LeaveRequestResponse MapToResponse(LeaveRequest request)
        {
            return new LeaveRequestResponse
            {
                LeaveRequestId = request.LeaveRequestId,
                EmployeeId = request.EmployeeId,
                LeaveTypeId = request.LeaveTypeId,
                LeaveTypeName = request.LeaveType?.Name ?? string.Empty,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalDays = request.TotalDays,
                Reason = request.Reason,
                Status = request.Status,
                ApprovedByEmployeeId = request.ApprovedByEmployeeId,
                RequestedAt = request.RequestedAt,
                ReviewedAt = request.ReviewedAt,
                RejectionReason = request.RejectionReason,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt
            };
        }
    }
}
