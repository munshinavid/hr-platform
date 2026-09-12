using System;
using System.Threading.Tasks;
using HRPlatform.Shared.Exceptions;
using LeaveManagement.Repository.Data;
using LeaveManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Repository.Implementations
{
    public class LeaveUnitOfWork : ILeaveUnitOfWork
    {
        private readonly LeaveDbContext _dbContext;

        public LeaveUnitOfWork(LeaveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await operation();

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();

                throw new ConcurrencyException(
                    "A concurrency conflict occurred.",
                    ex);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

