using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Orchestrator.DTO.Onboarding;
using Orchestrator.Handler.Onboarding;
using Orchestrator.DTO.Offboarding;
using Orchestrator.Handler.Offboarding;
using Orchestrator.DTO.EmployeeDashboard;
using Orchestrator.Handler.EmployeeDashboard;

namespace Orchestrator.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddOrchestratorHandlerLayer(
            this IServiceCollection services)
        {
            // Orchestration commands
            services.AddScoped<
                ICommandHandler<CreateEmployeeOnboardingCommand, HandlerResult<CreateEmployeeOnboardingResponse>>,
                CreateEmployeeOnboardingHandler>();

            services.AddScoped<
                ICommandHandler<OffboardEmployeeCommand, HandlerResult<OffboardEmployeeResponse>>,
                OffboardEmployeeHandler>();

            // Employee Dashboard Aggregation
            services.AddScoped<
                IQueryHandler<GetEmployeeDashboardQuery, HandlerResult<EmployeeDashboardResponse>>,
                GetEmployeeDashboardHandler>();

            // Dispatcher for Orchestrator.API → Orchestrator.Handler dispatch
            services.AddScoped<IDispatcher, Dispatcher>();

            // Safe Command Sender
            services.AddScoped<Infrastructure.SafeCommandSender>();

            return services;
        }
    }
}

