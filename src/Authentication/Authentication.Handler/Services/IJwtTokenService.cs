using Authentication.Aggregator.Entities;

namespace Authentication.Handler.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserAggregatorRoot user);

        int GetExpirationMinutes();
    }
}
