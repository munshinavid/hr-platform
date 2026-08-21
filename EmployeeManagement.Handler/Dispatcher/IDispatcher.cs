namespace EmployeeManagement.Handler.Dispatcher
{
    public interface IDispatcher
    {
        /// <summary>
        /// Resolves and invokes the registered <see cref="Abstractions.ICommandHandler{TCommand,TResult}"/>.
        /// </summary>
        Task<TResult> SendCommand<TCommand, TResult>(TCommand command, CancellationToken ct = default);

        /// <summary>
        /// Resolves and invokes the registered <see cref="Abstractions.IQueryHandler{TQuery,TResult}"/>.
        /// </summary>
        Task<TResult> SendQuery<TQuery, TResult>(TQuery query, CancellationToken ct = default);
    }
}
