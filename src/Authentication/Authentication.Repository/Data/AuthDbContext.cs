using Authentication.Aggregator.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Repository.Data
{
    /// <summary>
    /// EF Core DbContext for the Authentication bounded context.
    /// Maps the Authentication-scoped UserAggregatorRoot to the existing "User" table.
    ///
    /// The "Password" column stores BCrypt hashes. The property is named PasswordHash
    /// in the C# entity for clarity; HasColumnName maps it to the existing column name
    /// so no schema change or new migration is required.
    ///
    /// This context intentionally has NO knowledge of Employee, Department, or
    /// EmployeeManagement internals.
    /// </summary>
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAggregatorRoot> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAggregatorRoot>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.ToTable("User");

                // Map the C# property PasswordHash → existing column "Password"
                entity.Property(u => u.PasswordHash)
                    .HasColumnName("Password")
                    .IsRequired();
            });
        }
    }
}
