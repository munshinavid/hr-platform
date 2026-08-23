using EmployeeManagement.DTO.Employee;
using EmployeeManagement.Handler.Abstractions;
using EmployeeManagement.Handler.Commands.CreateEmployee;
using EmployeeManagement.Handler.Common;
using EmployeeManagement.Handler.Dispatcher;
using EmployeeManagement.Handler.Queries.GetEmployee;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHandlerLayer(this IServiceCollection services)
        {
            // Commands
            services.AddScoped<
                ICommandHandler<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>,
                CreateEmployeeHandler>();

            // Queries
            services.AddScoped<
                IQueryHandler<GetEmployeeQuery, HandlerResult<EmployeeResponse>>,
                GetEmployeeHandler>();

            // Dispatcher
            services.AddScoped<IDispatcher, Dispatcher.Dispatcher>();

            return services;
        }
    }
}
