using HRPlatform.Shared.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HRPlatform.Shared.Dispatcher
{
    public class Dispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public Dispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResult> SendCommand<TCommand, TResult>(TCommand command)
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
            return handler.HandleAsync(command);
        }

        public Task<TResult> SendQuery<TQuery, TResult>(TQuery query)
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            return handler.HandleAsync(query);
        }
    }
}
