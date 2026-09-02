using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace HRPlatform.Shared.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IServiceCollection AddFluentValidationConfiguration(
            this IServiceCollection services,
            Type validatorAssemblyMarker)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining(validatorAssemblyMarker);

            return services;
        }
    }
}
