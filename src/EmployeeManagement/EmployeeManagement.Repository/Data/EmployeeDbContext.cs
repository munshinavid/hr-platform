using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Aggregator.Aggregates;

namespace EmployeeManagement.Repository.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeAggregateRoot> Employees { get; set; }
        public DbSet<DepartmentAggregateRoot> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary Keys
            modelBuilder.Entity<EmployeeAggregateRoot>()
                .HasKey(e => e.EmployeeId);

            modelBuilder.Entity<DepartmentAggregateRoot>()
                .HasKey(d => d.DepartmentId);

            // Existing database table names
            modelBuilder.Entity<EmployeeAggregateRoot>()
                .ToTable("Employees");

            modelBuilder.Entity<DepartmentAggregateRoot>()
                .ToTable("Departments");

            // Employee salary
            modelBuilder.Entity<EmployeeAggregateRoot>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);

            // logical reference to the Identity User.
            modelBuilder.Entity<EmployeeAggregateRoot>()
                .Property(e => e.UserId)
                .IsRequired();

            // Self-referencing reporting manager
            modelBuilder.Entity<EmployeeAggregateRoot>()
                .HasOne(e => e.ReportingManager)
                .WithMany()
                .HasForeignKey(e => e.ReportingManagerId)
                .OnDelete(DeleteBehavior.Restrict);


            // Seed Department
            modelBuilder.Entity<DepartmentAggregateRoot>().HasData(
                new DepartmentAggregateRoot
                {
                    DepartmentId = 1,
                    DepartmentName = "IT"
                },
                new DepartmentAggregateRoot
                {
                    DepartmentId = 2,
                    DepartmentName = "HR"
                },
                new DepartmentAggregateRoot
                {
                    DepartmentId = 3,
                    DepartmentName = "Finance"
                }
            );

            modelBuilder.Entity<EmployeeAggregateRoot>().HasData(
                new
                {
                    EmployeeId = 1,
                    UserId = 3,
                    Name = "Rahim Ahmed",
                    Email = "rahim@gmail.com",
                    Phone = "01711111111",
                    Gender = "Male",
                    DepartmentId = 1,
                    JobTitle = "Software Engineer",
                    Salary = 45000m,
                    EmploymentType = "Full-Time",
                    JoiningDate = new DateTime(2025, 1, 10),
                    Status = "Active",
                    CreatedAt = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    EmployeeId = 2,
                    UserId = 4,
                    Name = "Karim Hasan",
                    Email = "karim@gmail.com",
                    Phone = "01722222222",
                    Gender = "Male",
                    DepartmentId = 2,
                    JobTitle = "HR Executive",
                    Salary = 35000m,
                    EmploymentType = "Full-Time",
                    JoiningDate = new DateTime(2024, 6, 15),
                    Status = "Active",
                    CreatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    EmployeeId = 3,
                    UserId = 5,
                    Name = "Nusrat Jahan",
                    Email = "nusrat@gmail.com",
                    Phone = "01733333333",
                    Gender = "Female",
                    DepartmentId = 3,
                    JobTitle = "Accountant",
                    Salary = 40000m,
                    EmploymentType = "Full-Time",
                    JoiningDate = new DateTime(2025, 3, 20),
                    Status = "Active",
                    CreatedAt = new DateTime(2025, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2025, 3, 20, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    EmployeeId = 4,
                    UserId = 6,
                    Name = "Hasan Mahmud",
                    Email = "hasan@gmail.com",
                    Phone = "01744444444",
                    Gender = "Male",
                    DepartmentId = 1,
                    JobTitle = "Web Developer",
                    Salary = 50000m,
                    EmploymentType = "Contract",
                    JoiningDate = new DateTime(2023, 9, 5),
                    Status = "Inactive",
                    CreatedAt = new DateTime(2023, 9, 5, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 9, 5, 0, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    EmployeeId = 5,
                    UserId = 2,
                    Name = "Sadia Akter",
                    Email = "sadia@gmail.com",
                    Phone = "01755555555",
                    Gender = "Female",
                    DepartmentId = 2,
                    JobTitle = "HR Manager",
                    Salary = 60000m,
                    EmploymentType = "Full-Time",
                    JoiningDate = new DateTime(2022, 11, 12),
                    Status = "Active",
                    CreatedAt = new DateTime(2022, 11, 12, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2022, 11, 12, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
