using IdentityManagement.Repository.Data;
using IdentityManagement.Repository.Implementations;
using IdentityManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityManagement.Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityRepositoryLayer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                ));

            services.AddScoped<IIdentityUserRepository, IdentityUserRepository>();

            return services;
        }
    }
}


