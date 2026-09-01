using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Query;
using EmployeeManagement.DTO.Response;
using EmployeeManagement.Handler.Commands.CreateEmployee;
using EmployeeManagement.Handler.Commands.UpdateEmployee;
using EmployeeManagement.Handler.Commands.Terminate;
using EmployeeManagement.Handler.Commands.AssignReportingManager;
using EmployeeManagement.Handler.Queries.GetEmployee;
using EmployeeManagement.Handler.Queries.GetEmployees;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EmployeeManagement.Repository;

namespace EmployeeManagement.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddEmployeeHandlerLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositoryLayer(configuration);
            // Commands
            services.AddScoped<
                ICommandHandler<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>,
                CreateEmployeeHandler>();
            services.AddScoped<
                ICommandHandler<UpdateEmployeeCommand, HandlerResult<EmployeeResponse>>,
                UpdateEmployeeHandler>();
            services.AddScoped<
                ICommandHandler<TerminateEmployeeCommand, HandlerResult>,
                TerminateEmployeeHandler>();
            services.AddScoped<
                ICommandHandler<AssignReportingManagerCommand, HandlerResult>,
                AssignReportingManagerHandler>();

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
