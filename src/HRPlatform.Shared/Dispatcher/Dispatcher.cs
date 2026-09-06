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

        public Task<TResponse> SendCommand<TCommand, TResponse>(TCommand command)
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
            return handler.HandleAsync(command);
        }

        public Task<TResponse> SendQuery<TQuery, TResponse>(TQuery query)
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
            return handler.HandleAsync(query);
        }
    }
}
