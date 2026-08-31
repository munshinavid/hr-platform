using HRPlatform.ServiceBus.Abstractions;
using HRPlatform.ServiceBus.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HRPlatform.ServiceBus.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services)
        {
            services.AddScoped<IServiceBus, InProcessServiceBus>();
            return services;
        }
    }
}
