using EmployeeManagement.Aggregator.Entities;

public class Employee
{
    public int EmployeeId { get; private set; }

    public string Phone { get; private set; } = string.Empty;

    public string Gender { get; private set; } = string.Empty;

    public int DepartmentId { get; private set; }

    public string JobTitle { get; private set; } = string.Empty;

    public decimal Salary { get; private set; }

    public string EmploymentType { get; private set; } = string.Empty;

    public DateTime JoiningDate { get; private set; }

    public string Status { get; private set; } = string.Empty;

    public int UserId { get; private set; }

    public Department? Department { get; private set; }

    public User User { get; private set; } = null!;

    private Employee(
        string phone,
        string gender,
        int departmentId,
        string jobTitle,
        decimal salary,
        string employmentType,
        DateTime joiningDate,
        string status,
        int userId)
    {
        if (salary < 0)
            throw new ArgumentException(
                "Employee salary cannot be negative.");

        if (joiningDate > DateTime.UtcNow)
            throw new ArgumentException(
                "Joining date cannot be in the future.");

        Phone = phone;
        Gender = gender;
        DepartmentId = departmentId;
        JobTitle = jobTitle;
        Salary = salary;
        EmploymentType = employmentType;
        JoiningDate = joiningDate;
        Status = status;
        UserId = userId;
    }

    public static Employee Create(
        string phone,
        string gender,
        int departmentId,
        string jobTitle,
        decimal salary,
        string employmentType,
        DateTime joiningDate,
        string status,
        int userId)
    {
        return new Employee(
            phone,
            gender,
            departmentId,
            jobTitle,
            salary,
            employmentType,
            joiningDate,
            status,
            userId);
    }
}