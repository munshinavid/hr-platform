using Authentication.Repository.Data;
using Authentication.Repository.Implementations;
using Authentication.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthRepositoryLayer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")
                ));

            services.AddScoped<IAuthUserRepository, AuthUserRepository>();

            return services;
        }
    }
}

