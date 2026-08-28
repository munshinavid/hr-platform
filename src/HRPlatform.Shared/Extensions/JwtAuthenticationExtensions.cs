using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HRPlatform.Shared.Extensions
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            var secretKey = jwtSettings.GetValue<string>("SecretKey")
                ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

            var issuer = jwtSettings.GetValue<string>("Issuer")
                ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");

            var audience = jwtSettings.GetValue<string>("Audience")
                ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer           = true,
                        ValidateAudience         = true,
                        ValidateLifetime         = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer              = issuer,
                        ValidAudience            = audience,
                        IssuerSigningKey         = key
                    };
                });

            return services;
        }
    }
}
