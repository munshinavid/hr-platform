using HRPlatform.ServiceBus.Abstractions;
using HRPlatform.Shared.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HRPlatform.ServiceBus.Implementations
{
    public sealed class InProcessServiceBus : IServiceBus
    {
        private readonly IServiceProvider _serviceProvider;

        public InProcessServiceBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<TResult> SendCommandAsync<TCommand, TResult>(TCommand command)
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResult>>()
                ?? throw new InvalidOperationException(
                    $"No handler registered for command '{typeof(TCommand).Name}'. " +
                    $"Ensure an ICommandHandler<{typeof(TCommand).Name}, {typeof(TResult).Name}> is registered in DI.");

            return handler.HandleAsync(command);
        }

        public Task<TResult> SendQueryAsync<TQuery, TResult>(TQuery query)
        {
            var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResult>>()
                ?? throw new InvalidOperationException(
                    $"No handler registered for query '{typeof(TQuery).Name}'. " +
                    $"Ensure an IQueryHandler<{typeof(TQuery).Name}, {typeof(TResult).Name}> is registered in DI.");

            return handler.HandleAsync(query);
        }
    }
}
