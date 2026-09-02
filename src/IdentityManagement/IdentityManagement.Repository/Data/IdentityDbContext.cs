using IdentityManagement.Aggregator.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityManagement.Repository.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
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
                entity.ToTable("Users");

                entity.Property(u => u.PasswordHash)
                    .HasColumnName("Password")
                    .IsRequired();


                // Account lifecycle column.
                // Migration default = 1 (true) so all pre-existing users remain active.
                entity.Property(u => u.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Audit columns.
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.UpdatedAt).IsRequired();

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                // Unique index on Email — fast lookup during login / duplicate check.
                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");
            });
        }
    }
}


