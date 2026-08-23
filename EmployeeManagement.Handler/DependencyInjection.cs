using EmployeeManagement.DTO.Common;
using EmployeeManagement.DTO.Employee;
using EmployeeManagement.Handler.Commands.CreateEmployee;
using EmployeeManagement.Handler.Common;
using EmployeeManagement.Handler.Queries.GetEmployee;
using EmployeeManagement.Handler.Queries.GetEmployees;
using EmployeeManagement.Shared.Abstractions;
using EmployeeManagement.Shared.Dispatcher;
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
            services.AddScoped<
                IQueryHandler<GetEmployeesQuery, HandlerResult<PagedResponse<EmployeeResponse>>>,
                GetEmployeesHandler>();

            // Dispatcher
            services.AddScoped<IDispatcher, Dispatcher>();

            return services;
        }
    }
}
