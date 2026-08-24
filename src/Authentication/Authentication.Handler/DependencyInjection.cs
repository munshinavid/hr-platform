using HRPlatform.Shared.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Handler
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers Authentication use-case handlers and the Dispatcher.
        /// Pattern mirrors EmployeeManagement.Handler.DependencyInjection.
        /// Future: register ICommandHandler<LoginCommand, HandlerResult<AuthResponse>>, LoginHandler.
        /// </summary>
        public static IServiceCollection AddAuthHandlerLayer(this IServiceCollection services)
        {
            // Dispatcher is registered here (or could be registered once in a shared bootstrap).
            // If the Authentication API is a standalone entry point it needs its own dispatcher.
            services.AddScoped<IDispatcher, Dispatcher>();

            // TODO: Register Login command handler when implemented.

            return services;
        }
    }
}
