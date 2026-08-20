namespace EmployeeManagement.Aggregator.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public string JobTitle { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public string EmploymentType { get; set; } = string.Empty;

        public DateTime JoiningDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public int UserId { get; set; }

        // Navigation
        public Department? Department { get; set; }
        public User User { get; set; } = null!;
    }
}
