using EmployeeManagement.Repository.Data;
using EmployeeManagement.Repository.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace EmployeeManagement.Repository.Implementations
{
    public class TransactionManager : ITransactionManager
    {
        private readonly EmployeeDbContext _context;
        private IDbContextTransaction? _transaction;

        public TransactionManager(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction =
                await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}