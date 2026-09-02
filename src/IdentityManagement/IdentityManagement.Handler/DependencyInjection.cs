using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using IdentityManagement.Aggregator.Validation;
using IdentityManagement.DTO.Command;
using IdentityManagement.DTO.Query;
using IdentityManagement.DTO.Response;
using IdentityManagement.Handler.Commands.Activate;
using IdentityManagement.Handler.Commands.Deactivate;
using IdentityManagement.Handler.Commands.Login;
using IdentityManagement.Handler.Commands.Register;
using IdentityManagement.Handler.Queries.GetUserProfile;
using IdentityManagement.Handler.Queries.GetUserStatus;
using IdentityManagement.Handler.Services;
using IdentityManagement.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HRPlatform.Shared.Extensions;

namespace IdentityManagement.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityHandlerLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // FluentValidation 
            services.AddFluentValidationConfiguration(typeof(RegisterUserCommandValidator));

            services.AddIdentityRepositoryLayer(configuration);

            // ── Services 
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // ── Commands — existing 

            services.AddScoped<
                ICommandHandler<RegisterUserCommand, HandlerResult>,
                RegisterUserHandler>();

            services.AddScoped<
                ICommandHandler<RegisterUserCommand, HandlerResult<UserRegistrationResult>>,
                RegisterUserWithResultHandler>();

            services.AddScoped<
                ICommandHandler<LoginCommand, HandlerResult<IdentityResponse>>,
                LoginHandler>();

            services.AddScoped<
                ICommandHandler<DeleteUserCommand, HandlerResult>,
                Commands.Delete.DeleteUserHandler>();

            //  Commands — account

            services.AddScoped<
                ICommandHandler<DeactivateUserCommand, HandlerResult>,
                DeactivateUserHandler>();

            services.AddScoped<
                ICommandHandler<ActivateUserCommand, HandlerResult>,
                ActivateUserHandler>();

            //  Queries — account  

            services.AddScoped<
                IQueryHandler<GetUserStatusQuery, HandlerResult<UserStatusResponse>>,
                GetUserStatusHandler>();

            services.AddScoped<
                IQueryHandler<GetUserProfileQuery, HandlerResult<UserProfileResponse>>,
                GetUserProfileHandler>();

            //  Dispatcher 
            services.AddScoped<IDispatcher, Dispatcher>();

            return services;
        }
    }
}




