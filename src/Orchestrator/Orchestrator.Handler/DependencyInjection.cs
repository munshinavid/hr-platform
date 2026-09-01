using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Orchestrator.DTO.Onboarding;
using Orchestrator.Handler.Onboarding;
using Orchestrator.DTO.Offboarding;
using Orchestrator.Handler.Offboarding;

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

            // Dispatcher for Orchestrator.API → Orchestrator.Handler dispatch
            services.AddScoped<IDispatcher, Dispatcher>();

            return services;
        }
    }
}

