using IdentityManagement.DTO.Command;
using IdentityManagement.DTO.Response;
using IdentityManagement.Handler.Commands.Login;
using IdentityManagement.Handler.Commands.Register;
using IdentityManagement.Handler.Services;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityManagement.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityHandlerLayer(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Commands
            services.AddScoped<
                ICommandHandler<RegisterUserCommand, HandlerResult>,
                RegisterUserHandler>();

            services.AddScoped<
                ICommandHandler<LoginCommand, HandlerResult<IdentityResponse>>,
                LoginHandler>();

            // Dispatcher (registered per-API entry point, mirrors EM.Handler convention)
            services.AddScoped<IDispatcher, Dispatcher>();

            return services;
        }
    }
}



