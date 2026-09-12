using LeaveManagement.Aggregator.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Repository.Data
{
    public class LeaveDbContext : DbContext
    {
        public LeaveDbContext(DbContextOptions<LeaveDbContext> options) : base(options)
        {
        }

        public DbSet<LeaveTypeAggregateRoot> LeaveTypes { get; set; }
        public DbSet<LeaveBalanceAggregateRoot> LeaveBalances { get; set; }
        public DbSet<LeaveRequestAggregateRoot> LeaveRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // LeaveType configuration
            modelBuilder.Entity<LeaveTypeAggregateRoot>()
                .HasKey(lt => lt.LeaveTypeId);
            
            // Seed base leave types
            modelBuilder.Entity<LeaveTypeAggregateRoot>().HasData(
                new LeaveTypeAggregateRoot { LeaveTypeId = 1, Name = "Annual Leave", Code = "ANNUAL", Description = "Standard annual leave", DefaultDaysPerYear = 20, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new LeaveTypeAggregateRoot { LeaveTypeId = 2, Name = "Sick Leave", Code = "SICK", Description = "Standard sick leave", DefaultDaysPerYear = 10, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new LeaveTypeAggregateRoot { LeaveTypeId = 3, Name = "Casual Leave", Code = "CASUAL", Description = "Casual / personal leave", DefaultDaysPerYear = 5, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            // LeaveBalance configuration
            modelBuilder.Entity<LeaveBalanceAggregateRoot>()
                .HasKey(lb => lb.LeaveBalanceId);

            modelBuilder.Entity<LeaveBalanceAggregateRoot>()
                .HasIndex(lb => new { lb.EmployeeId, lb.LeaveTypeId, lb.Year })
                .IsUnique();

            modelBuilder.Entity<LeaveBalanceAggregateRoot>()
                .Property(lb => lb.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            modelBuilder.Entity<LeaveBalanceAggregateRoot>()
                .HasOne(lb => lb.LeaveType)
                .WithMany()
                .HasForeignKey(lb => lb.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // LeaveRequest configuration
            modelBuilder.Entity<LeaveRequestAggregateRoot>()
                .HasKey(lr => lr.LeaveRequestId);

            modelBuilder.Entity<LeaveRequestAggregateRoot>()
                .HasOne(lr => lr.LeaveType)
                .WithMany()
                .HasForeignKey(lr => lr.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seeding LeaveBalances for existing seeded employees (Year 2026)
            modelBuilder.Entity<LeaveBalanceAggregateRoot>().HasData(
                // Rahim Ahmed (EmployeeId = 1)
                new LeaveBalanceAggregateRoot
                {
                    LeaveBalanceId = 1,
                    EmployeeId = 1,
                    LeaveTypeId = 1, // Annual Leave
                    Year = 2026,
                    TotalDays = 20,
                    HeldDays = 0,
                    UsedDays = 0,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new LeaveBalanceAggregateRoot
                {
                    LeaveBalanceId = 2,
                    EmployeeId = 1,
                    LeaveTypeId = 2, // Sick Leave
                    Year = 2026,
                    TotalDays = 10,
                    HeldDays = 0,
                    UsedDays = 0,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new LeaveBalanceAggregateRoot
                {
                    LeaveBalanceId = 3,
                    EmployeeId = 1,
                    LeaveTypeId = 3, // Casual Leave
                    Year = 2026,
                    TotalDays = 5,
                    HeldDays = 0,
                    UsedDays = 0,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },

                // Karim Hasan (EmployeeId = 2)
                new LeaveBalanceAggregateRoot
                {
                    LeaveBalanceId = 4,
                    EmployeeId = 2,
                    LeaveTypeId = 1,
                    Year = 2026,
                    TotalDays = 20,
                    HeldDays = 0,
                    UsedDays = 2,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },

                // Sadia Akter (EmployeeId = 5 - HR Manager)
                new LeaveBalanceAggregateRoot
                {
                    LeaveBalanceId = 5,
                    EmployeeId = 5,
                    LeaveTypeId = 1,
                    Year = 2026,
                    TotalDays = 20,
                    HeldDays = 0,
                    UsedDays = 0,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );


            modelBuilder.Entity<LeaveRequestAggregateRoot>().HasData(
                new
                {
                    LeaveRequestId = 1,
                    EmployeeId = 1,
                    LeaveTypeId = 1,
                    StartDate = new DateTime(2026, 10, 5),
                    EndDate = new DateTime(2026, 10, 7),
                    TotalDays = 3,
                    Reason = "Family vacation",
                    Status = "Pending",
                    RequestedAt = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc),
                    ApprovedByEmployeeId = (int?)null,
                    ReviewedAt = (DateTime?)null,
                    RejectionReason = (string?)null,
                    CreatedAt = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    LeaveRequestId = 2,
                    EmployeeId = 2,
                    LeaveTypeId = 1,
                    StartDate = new DateTime(2026, 8, 10),
                    EndDate = new DateTime(2026, 8, 11),
                    TotalDays = 2,
                    Reason = "Personal work",
                    Status = "Approved",
                    RequestedAt = new DateTime(2026, 8, 1, 9, 0, 0, DateTimeKind.Utc),
                    ApprovedByEmployeeId = (int?)5,
                    ReviewedAt = (DateTime?)new DateTime(2026, 8, 2, 14, 0, 0, DateTimeKind.Utc),
                    RejectionReason = (string?)null,
                    CreatedAt = new DateTime(2026, 8, 1, 9, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 8, 2, 14, 0, 0, DateTimeKind.Utc)
                },
                new
                {
                    LeaveRequestId = 3,
                    EmployeeId = 1,
                    LeaveTypeId = 3,
                    StartDate = new DateTime(2026, 7, 1),
                    EndDate = new DateTime(2026, 7, 2),
                    TotalDays = 2,
                    Reason = "Urgent travel",
                    Status = "Rejected",
                    RequestedAt = new DateTime(2026, 6, 25, 11, 0, 0, DateTimeKind.Utc),
                    ApprovedByEmployeeId = (int?)5,
                    ReviewedAt = (DateTime?)new DateTime(2026, 6, 26, 16, 0, 0, DateTimeKind.Utc),
                    RejectionReason = "Critical release scheduled on those dates",
                    CreatedAt = new DateTime(2026, 6, 25, 11, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 6, 26, 16, 0, 0, DateTimeKind.Utc)
                }
            );


        }
    }
}
