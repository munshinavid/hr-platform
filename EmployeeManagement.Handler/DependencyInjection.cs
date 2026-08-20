using EmployeeManagement.Handler.Commands.CreateEmployee;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHandlerLayer(this IServiceCollection services)
        {
            services.AddScoped<CreateEmployeeHandler>();

            return services;
        }
    }
}
