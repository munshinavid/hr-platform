using System;
using System.Threading.Tasks;

namespace HRPlatform.Shared.Abstractions
{
    public interface IUnitOfWork
    {
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}

