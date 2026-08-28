using Authentication.Aggregator.Entities;

namespace Authentication.Handler.Services
{
    public interface IJwtTokenService
    {
        /// <summary>Generates a signed JWT for the given user.</summary>
        string GenerateToken(UserAggregatorRoot user);

        /// <summary>Returns the configured token expiration in minutes.</summary>
        int GetExpirationMinutes();
    }
}
