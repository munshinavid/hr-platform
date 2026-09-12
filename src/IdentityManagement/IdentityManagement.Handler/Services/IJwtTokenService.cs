using IdentityManagement.Aggregator.Aggregates;

namespace IdentityManagement.Handler.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserAggregateRoot user);

        int GetExpirationMinutes();
    }
}

