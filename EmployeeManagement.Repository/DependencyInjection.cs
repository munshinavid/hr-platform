using EmployeeManagement.Repository.Data;
using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Repository.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositoryLayer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<EmployeeDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                ));

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ITransactionManager, TransactionManager>();

            return services;
        }
    }
}
