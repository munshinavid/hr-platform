using IdentityManagement.Aggregator.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace IdentityManagement.Repository.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAggregateRoot> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserAggregateRoot>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.ToTable("Users");

                entity.Property(u => u.PasswordHash)
                    .HasColumnName("Password")
                    .IsRequired();


                entity.Property(u => u.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.UpdatedAt).IsRequired();

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                // Unique index on Email for fast lookup 
                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");
            });
        }
    }
}


