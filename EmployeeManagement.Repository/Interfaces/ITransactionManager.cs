namespace EmployeeManagement.Repository.Interfaces
{
    public interface ITransactionManager
    {
        Task BeginTransactionAsync();

        Task CommitAsync();

        Task RollbackAsync();
    }
}