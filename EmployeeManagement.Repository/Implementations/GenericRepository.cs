using EmployeeManagement.Repository.Interfaces;
using EmployeeManagement.Aggregator.Entities;   
using EmployeeManagement.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Repository.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly EmployeeDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(EmployeeDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<bool> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
