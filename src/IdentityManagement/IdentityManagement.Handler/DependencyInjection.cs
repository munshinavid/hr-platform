using IdentityManagement.DTO.Command;
using IdentityManagement.DTO.Response;
using IdentityManagement.Handler.Commands.Login;
using IdentityManagement.Handler.Commands.Register;
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
            // Services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Commands — original handler (void result) for IdentityController direct endpoint
            services.AddScoped<
                ICommandHandler<RegisterUserCommand, HandlerResult>,
                RegisterUserHandler>();

            // Commands — orchestration-facing handler (returns UserId) for ServiceBus orchestration
            services.AddScoped<
                ICommandHandler<RegisterUserCommand, HandlerResult<UserRegistrationResult>>,
                RegisterUserWithResultHandler>();

            services.AddScoped<
                ICommandHandler<LoginCommand, HandlerResult<IdentityResponse>>,
                LoginHandler>();

            services.AddScoped<
                ICommandHandler<DeleteUserCommand, HandlerResult>,
                IdentityManagement.Handler.Commands.Delete.DeleteUserHandler>();

            // Dispatcher (registered per-API entry point, mirrors EM.Handler convention)
            services.AddScoped<IDispatcher, Dispatcher>();

            return services;
        }
    }
}




