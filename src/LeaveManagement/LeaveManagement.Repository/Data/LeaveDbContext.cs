using LeaveManagement.Aggregator.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Repository.Data
{
    public class LeaveDbContext : DbContext
    {
        public LeaveDbContext(DbContextOptions<LeaveDbContext> options) : base(options)
        {
        }

        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // LeaveType configuration
            modelBuilder.Entity<LeaveType>()
                .HasKey(lt => lt.LeaveTypeId);
            
            // Seed base leave types
            modelBuilder.Entity<LeaveType>().HasData(
                new LeaveType { LeaveTypeId = 1, Name = "Annual Leave", Code = "ANNUAL", Description = "Standard annual leave", DefaultDaysPerYear = 20, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new LeaveType { LeaveTypeId = 2, Name = "Sick Leave", Code = "SICK", Description = "Standard sick leave", DefaultDaysPerYear = 10, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new LeaveType { LeaveTypeId = 3, Name = "Casual Leave", Code = "CASUAL", Description = "Casual / personal leave", DefaultDaysPerYear = 5, IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            // LeaveBalance configuration
            modelBuilder.Entity<LeaveBalance>()
                .HasKey(lb => lb.LeaveBalanceId);

            modelBuilder.Entity<LeaveBalance>()
                .HasIndex(lb => new { lb.EmployeeId, lb.LeaveTypeId, lb.Year })
                .IsUnique();

            modelBuilder.Entity<LeaveBalance>()
                .Property(lb => lb.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            modelBuilder.Entity<LeaveBalance>()
                .HasOne(lb => lb.LeaveType)
                .WithMany()
                .HasForeignKey(lb => lb.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // LeaveRequest configuration
            modelBuilder.Entity<LeaveRequest>()
                .HasKey(lr => lr.LeaveRequestId);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.LeaveType)
                .WithMany()
                .HasForeignKey(lr => lr.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
