using Authentication.DTO.Command;
using Authentication.DTO.Response;
using Authentication.Handler.Commands.Login;
using Authentication.Handler.Commands.Register;
using Authentication.Handler.Services;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthHandlerLayer(this IServiceCollection services)
        {
            // Services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Commands
            services.AddScoped<
                ICommandHandler<RegisterUserCommand, HandlerResult>,
                RegisterUserHandler>();

            services.AddScoped<
                ICommandHandler<LoginCommand, HandlerResult<AuthResponse>>,
                LoginHandler>();

            // Dispatcher (registered per-API entry point, mirrors EM.Handler convention)
            services.AddScoped<IDispatcher, Dispatcher>();

            return services;
        }
    }
}

