using HRPlatform.ServiceBus.Abstractions;
using HRPlatform.ServiceBus.Implementations;
using Microsoft.Extensions.DependencyInjection;

using EmployeeManagement.Handler;
using IdentityManagement.Handler;
using LeaveManagement.Handler;
using Microsoft.Extensions.Configuration;

namespace HRPlatform.ServiceBus.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IServiceBus, InProcessServiceBus>();
            
            // Register subsystem handlers here
            services.AddIdentityHandlerLayer(configuration);
            services.AddEmployeeHandlerLayer(configuration);
            services.AddLeaveHandlerLayer(configuration);
            
            return services;
        }
    }
}
