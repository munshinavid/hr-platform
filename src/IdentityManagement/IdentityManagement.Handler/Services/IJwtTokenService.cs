using IdentityManagement.Aggregator.Entities;

namespace IdentityManagement.Handler.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserAggregatorRoot user);

        int GetExpirationMinutes();
    }
}

