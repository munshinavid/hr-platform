using Authentication.Aggregator.Entities;

namespace Authentication.Repository.Interfaces
{
    /// <summary>
    /// Repository contract for AuthUser persistence in the Authentication bounded context.
    /// Do NOT use EmployeeManagement repositories here.
    /// </summary>
    public interface IAuthUserRepository
    {
        Task<AuthUser?> GetByEmailAsync(string email);
    }
}
