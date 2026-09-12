using System;
using LeaveManagement.Aggregator.Exceptions;

namespace LeaveManagement.Aggregator.Aggregates
{
    public class LeaveRequestAggregateRoot
    {
        public int LeaveRequestId { get; set; }
        public int EmployeeId { get; set; }
        
        public int LeaveTypeId { get; set; }
        public LeaveTypeAggregateRoot? LeaveType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; } = string.Empty;
        
        public string Status { get; private set; } = "Pending";
        
        public int? ApprovedByEmployeeId { get; private set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ReviewedAt { get; private set; }
        public string? RejectionReason { get; private set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static LeaveRequestAggregateRoot Apply(int employeeId, int leaveTypeId, DateTime startDate, DateTime endDate, int totalDays, string reason)
        {
            if (totalDays <= 0)
                throw new DomainException("Total days must be positive.");
            if (startDate > endDate)
                throw new DomainException("Start date cannot be after end date.");

            var now = DateTime.UtcNow;
            return new LeaveRequestAggregateRoot
            {
                EmployeeId = employeeId,
                LeaveTypeId = leaveTypeId,
                StartDate = startDate,
                EndDate = endDate,
                TotalDays = totalDays,
                Reason = reason,
                Status = "Pending",
                RequestedAt = now,
                CreatedAt = now,
                UpdatedAt = now
            };
        }

        public void Approve(int approvedByEmployeeId)
        {
            if (Status != "Pending")
                throw new DomainException($"Cannot approve request in '{Status}' state.");

            Status = "Approved";
            ApprovedByEmployeeId = approvedByEmployeeId;
            ReviewedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reject(int rejectedByEmployeeId, string reason)
        {
            if (Status != "Pending")
                throw new DomainException($"Cannot reject request in '{Status}' state.");
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Rejection reason must be provided.");

            Status = "Rejected";
            RejectionReason = reason;
            ApprovedByEmployeeId = rejectedByEmployeeId;       
            ReviewedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status != "Pending")
                throw new DomainException($"Cannot cancel request in '{Status}' state. Only Pending requests can be cancelled.");

            Status = "Cancelled";
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
