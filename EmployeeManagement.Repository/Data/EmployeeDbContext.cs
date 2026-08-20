using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Aggregator.Entities;

namespace EmployeeManagement.Repository.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, DepartmentName = "IT" },
                new Department { DepartmentId = 2, DepartmentName = "HR" },
                new Department { DepartmentId = 3, DepartmentName = "Finance" }
            );

            // Reusing a default hashed password for seeded users
            string defaultPasswordHash = "$2a$11$Xlrxgz54nA19DFt72QE8KObUgjYvFwMOd0WptsY59zuA60LMcP8YW"; // "password123" maybe?

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Name = "Admin",
                    Email = "navid@gmail.com",
                    Password = "$2a$11$Yf0Mi/zOVDnkRqRHvOOin.MtWb.w36EGmQW/f55XQ5yvz51uBBXU6",
                    Role = "HR"
                },
                new User
                {
                    UserId = 2,
                    Name = "Sadia Akter", // was Test User
                    Email = "sadia@gmail.com", // was user@gmail.com
                    Password = defaultPasswordHash,
                    Role = "Employee"
                },
                new User
                {
                    UserId = 3,
                    Name = "Rahim Ahmed",
                    Email = "rahim@gmail.com",
                    Password = defaultPasswordHash,
                    Role = "Employee"
                },
                new User
                {
                    UserId = 4,
                    Name = "Karim Hasan",
                    Email = "karim@gmail.com",
                    Password = defaultPasswordHash,
                    Role = "Employee"
                },
                new User
                {
                    UserId = 5,
                    Name = "Nusrat Jahan",
                    Email = "nusrat@gmail.com",
                    Password = defaultPasswordHash,
                    Role = "Employee"
                },
                new User
                {
                    UserId = 6,
                    Name = "Hasan Mahmud",
                    Email = "hasan@gmail.com",
                    Password = defaultPasswordHash,
                    Role = "Employee"
                }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    UserId = 3,
                    Phone = "01711111111",
                    Gender = "Male",
                    DepartmentId = 1,
                    JobTitle = "Software Engineer",
                    Salary = 45000,
                    EmploymentType = "Full-Time",
                    JoiningDate = new DateTime(2025, 1, 10),
                    Status = "Active"
                },
                new Employee
                {
                    EmployeeId = 2,
                    UserId = 4,
                    Phone = "01722222222",
                    Gender = "Male",
                    DepartmentId = 2,
                    JobTitle = "HR Executive",
                    Salary = 35000,
                    EmploymentType = "Full-Time",
                    JoiningDate = new DateTime(2024, 6, 15),
                    Status = "Active"
                },
                new Employee
                {
                    EmployeeId = 3,
                    UserId = 5,
                    Phone = "01733333333",
                    Gender = "Female",
                    DepartmentId = 3,
                    JobTitle = "Accountant",
                    Salary = 40000,
                    EmploymentType = "Full-Time",
                    JoiningDate = new DateTime(2025, 3, 20),
                    Status = "Active"
                },
                new Employee
                {
                    EmployeeId = 4,
                    UserId = 6,
                    Phone = "01744444444",
                    Gender = "Male",
                    DepartmentId = 1,
                    JobTitle = "Web Developer",
                    Salary = 50000,
                    EmploymentType = "Contract",
                    JoiningDate = new DateTime(2023, 9, 5),
                    Status = "Inactive"
                },
                new Employee
                {
                    EmployeeId = 5,
                    UserId = 2,
                    Phone = "01755555555",
                    Gender = "Female",
                    DepartmentId = 2,
                    JobTitle = "HR Manager",
                    Salary = 60000,
                    EmploymentType = "Full-Time",
                    JoiningDate = new DateTime(2022, 11, 12),
                    Status = "Active"
                }
            );
        }
    }
}
