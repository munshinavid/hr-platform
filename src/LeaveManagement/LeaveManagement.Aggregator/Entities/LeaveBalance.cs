using System;
using LeaveManagement.Aggregator.Exceptions;

namespace LeaveManagement.Aggregator.Entities
{
    public class LeaveBalance
    {
        public int LeaveBalanceId { get; set; }
        
        // Scalar reference to EmployeeManagement. No navigation property.
        public int EmployeeId { get; set; }
        
        public int LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get; set; }
        
        public int Year { get; set; }
        public int TotalDays { get; set; }
        public int UsedDays { get; set; }
        public int HeldDays { get; set; }
        
        // Concurrency token
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int AvailableDays => TotalDays - UsedDays - HeldDays;

        public void Hold(int days)
        {
            if (days <= 0)
                throw new DomainException("Days to hold must be positive.");

            if (AvailableDays < days)
                throw new DomainException("Insufficient leave balance available.");

            HeldDays += days;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReleaseHold(int days)
        {
            if (days <= 0)
                throw new DomainException("Days to release must be positive.");

            if (HeldDays < days)
                throw new DomainException("Cannot release more days than are currently held.");

            HeldDays -= days;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UseHold(int days)
        {
            if (days <= 0)
                throw new DomainException("Days to use must be positive.");

            if (HeldDays < days)
                throw new DomainException("Cannot use more days than are currently held.");

            HeldDays -= days;
            UsedDays += days;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
