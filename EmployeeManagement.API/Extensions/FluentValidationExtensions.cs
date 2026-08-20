using EmployeeManagement.Handler.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace EmployeeManagement.API.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IServiceCollection AddFluentValidationConfiguration(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining<CreateEmployeeRequestValidator>();

            return services;
        }
    }
}
