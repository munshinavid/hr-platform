namespace EmployeeManagement.Shared.Dispatcher
{
    public interface IDispatcher
    {
        Task<TResult> SendCommand<TCommand, TResult>(TCommand command);

        Task<TResult> SendQuery<TQuery, TResult>(TQuery query);
    }
}
