using IdentityManagement.DTO.Command;
using IdentityManagement.DTO.Query;
using IdentityManagement.DTO.Response;
using IdentityManagement.Handler.Commands.Login;
using IdentityManagement.Handler.Commands.Register;
using IdentityManagement.Handler.Commands.Deactivate;
using IdentityManagement.Handler.Commands.Activate;
using IdentityManagement.Handler.Queries.GetUserStatus;
using IdentityManagement.Handler.Queries.GetUserProfile;
using IdentityManagement.Handler.Services;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using IdentityManagement.Repository;

namespace IdentityManagement.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityHandlerLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityRepositoryLayer(configuration);

            // ── Services ─────────────────────────────────────────────────────────
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // ── Commands — existing ───────────────────────────────────────────────

            // Void result: used directly by IdentityController /register endpoint.
            services.AddScoped<
                ICommandHandler<RegisterUserCommand, HandlerResult>,
                RegisterUserHandler>();

            // Rich result: used by Orchestrator ServiceBus for onboarding (returns UserId).
            services.AddScoped<
                ICommandHandler<RegisterUserCommand, HandlerResult<UserRegistrationResult>>,
                RegisterUserWithResultHandler>();

            services.AddScoped<
                ICommandHandler<LoginCommand, HandlerResult<IdentityResponse>>,
                LoginHandler>();

            // Hard delete — used as compensation by Onboarding Orchestrator.
            services.AddScoped<
                ICommandHandler<DeleteUserCommand, HandlerResult>,
                IdentityManagement.Handler.Commands.Delete.DeleteUserHandler>();

            // ── Commands — account lifecycle (Phase A) ────────────────────────────

            // Soft deactivation — used by Offboarding Orchestrator (Phase D).
            services.AddScoped<
                ICommandHandler<DeactivateUserCommand, HandlerResult>,
                DeactivateUserHandler>();

            // Re-activation — compensation counterpart for offboarding rollback.
            services.AddScoped<
                ICommandHandler<ActivateUserCommand, HandlerResult>,
                ActivateUserHandler>();

            // ── Queries — account lifecycle (Phase A) ─────────────────────────────

            // Lightweight gatekeeper: is this account active?
            services.AddScoped<
                IQueryHandler<GetUserStatusQuery, HandlerResult<UserStatusResponse>>,
                GetUserStatusHandler>();

            // Full identity profile for Orchestrator composite reads (Employee Dashboard).
            services.AddScoped<
                IQueryHandler<GetUserProfileQuery, HandlerResult<UserProfileResponse>>,
                GetUserProfileHandler>();

            // ── Dispatcher (per-API-host, mirrors EM.Handler convention) ─────────
            services.AddScoped<IDispatcher, Dispatcher>();

            return services;
        }
    }
}




