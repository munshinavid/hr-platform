using EmployeeManagement.Handler.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagement.Handler.Dispatcher
{
    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public Dispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResult> SendCommand<TCommand, TResult>(TCommand command, CancellationToken ct = default)
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            return handler.HandleAsync(command, ct);
        }

        public Task<TResult> SendQuery<TQuery, TResult>(TQuery query, CancellationToken ct = default)
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return handler.HandleAsync(query, ct);
        }
    }
}
