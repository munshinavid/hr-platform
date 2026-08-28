using Authentication.Aggregator.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Handler.Services
{
    /// <summary>
    /// JWT token generation service.
    /// Reads Jwt:SecretKey, Jwt:Issuer, Jwt:Audience, Jwt:ExpirationMinutes from configuration.
    /// Claims follow the same pattern used in the 3-tier AuthService:
    ///   - ClaimTypes.NameIdentifier → UserId
    ///   - ClaimTypes.Email          → Email
    ///   - ClaimTypes.Role           → Role
    ///
    /// Uses System.IdentityModel.Tokens.Jwt for token creation.
    /// JWT Bearer validation is configured separately in HRPlatform.Shared.Extensions.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserAggregatorRoot user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            var secretKey = jwtSettings.GetValue<string>("SecretKey")
                ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

            var issuer = jwtSettings.GetValue<string>("Issuer")
                ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");

            var audience = jwtSettings.GetValue<string>("Audience")
                ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

            var expirationMinutes = jwtSettings.GetValue<int>("ExpirationMinutes");

            var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email,           user.Email),
                new Claim(ClaimTypes.Role,            user.Role)
            };

            var token = new JwtSecurityToken(
                issuer:            issuer,
                audience:          audience,
                claims:            claims,
                expires:           DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int GetExpirationMinutes()
        {
            return _configuration.GetSection("Jwt").GetValue<int>("ExpirationMinutes");
        }
    }
}
