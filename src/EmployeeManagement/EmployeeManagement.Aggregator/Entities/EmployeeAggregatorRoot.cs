using EmployeeManagement.Aggregator.Exceptions;
using EmployeeManagement.Aggregator.Mapping;
using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Response;

namespace EmployeeManagement.Aggregator.Entities
{
    public class EmployeeAggregatorRoot
    {
        public int EmployeeId { get; set; }

        // Logical reference to the Identity User — no EF navigation property.
        // Employee.UserId is a plain scalar FK; synchronisation of Name/Email
        // is handled by Phase 2 (ServiceBus / domain events).
        public int UserId { get; set; }

        // Employee-owned, denormalized copy of the identity name/email.
        // Source of truth for Name/Email is User in the Authentication context.
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public string EmploymentType { get; set; } = string.Empty;

        public DateTime JoiningDate { get; set; }

        public string Status { get; set; } = string.Empty;

        // Department navigation is kept — Department is owned by EmployeeManagement.
        public DepartmentAggregatorRoot? Department { get; set; }

        public int? ReportingManagerId { get; set; }
        public EmployeeAggregatorRoot? ReportingManager { get; set; }

        public DateTime? TerminationDate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public static void ValidateBusinessRules(
            decimal salary,
            DateTime joiningDate)
        {
            if (salary < 0)
                throw new DomainException(
                    "Employee salary cannot be negative.");

            if (joiningDate > DateTime.UtcNow)
                throw new DomainException(
                    "Joining date cannot be in the future.");
        }

        public static EmployeeAggregatorRoot MapToAggregator(
            CreateEmployeeCommand command,
            int userId)
        {
            ValidateBusinessRules(
                command.Salary,
                command.JoiningDate);

            return EmployeeMapper.MapToAggregator(
                command,
                userId);
        }

        public void MapToAggregator(
            UpdateEmployeeCommand command)
        {
            ValidateBusinessRules(
                command.Salary,
                command.JoiningDate);

            EmployeeMapper.MapToAggregator(
                this,
                command);
        }

        public EmployeeResponse MapToResponse()
        {
            return EmployeeResponseMapper.MapToResponse(this);
        }

        public void Terminate()
        {
            if (Status == "Terminated")
                throw new DomainException("Employee is already terminated.");

            Status = "Terminated";
            TerminationDate = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            if (Status != "Terminated")
                throw new DomainException("Employee is not terminated and therefore cannot be reactivated.");

            Status = "Active";
            TerminationDate = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignReportingManager(int reportingManagerId)
        {
            if (reportingManagerId == EmployeeId)
                throw new DomainException("An employee cannot report to themselves.");

            ReportingManagerId = reportingManagerId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}