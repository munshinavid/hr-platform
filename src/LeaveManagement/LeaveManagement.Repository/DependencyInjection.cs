using LeaveManagement.Repository.Data;
using LeaveManagement.Repository.Implementations;
using LeaveManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagement.Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositoryLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LeaveDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(LeaveDbContext).Assembly.FullName)));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<ILeaveBalanceRepository, LeaveBalanceRepository>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            
            // Register DbContext as the unit of work / context provider if needed, or rely on scoped dependencies.
            return services;
        }
    }
}
