using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using Orchestrator.DTO.Onboarding;
using Orchestrator.Handler.Onboarding;

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

            // Dispatcher for Orchestrator.API → Orchestrator.Handler dispatch
            services.AddScoped<IDispatcher, Dispatcher>();

            return services;
        }
    }
}

