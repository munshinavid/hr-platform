using Authentication.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.Repository
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers Authentication persistence infrastructure.
        /// Future: register AuthDbContext, IAuthUserRepository implementation, etc.
        /// </summary>
        public static IServiceCollection AddAuthRepositoryLayer(
            this IServiceCollection services)
        {
            // TODO: Register AuthDbContext and IAuthUserRepository implementation
            // when the Login feature is implemented.

            return services;
        }
    }
}
