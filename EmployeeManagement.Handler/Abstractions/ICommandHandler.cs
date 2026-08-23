namespace EmployeeManagement.Handler.Abstractions
{
    public interface ICommandHandler<TCommand, TResult>
    {
        Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
    }
}
