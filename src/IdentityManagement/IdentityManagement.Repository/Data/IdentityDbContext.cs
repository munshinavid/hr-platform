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

                // Map the C# property PasswordHash → existing column "Password"
                entity.Property(u => u.PasswordHash)
                    .HasColumnName("Password")
                    .IsRequired();
            });
        }
    }
}

