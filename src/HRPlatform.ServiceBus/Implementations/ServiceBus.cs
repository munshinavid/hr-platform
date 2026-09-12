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

        public Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command)
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResponse>>()
                ?? throw new InvalidOperationException(
                    $"No handler registered for command '{typeof(TCommand).Name}'. " +
                    $"Ensure an ICommandHandler<{typeof(TCommand).Name}, {typeof(TResponse).Name}> is registered in DI.");

            return handler.HandleAsync(command);
        }

        public Task<TResponse> SendQueryAsync<TQuery, TResponse>(TQuery query)
        {
            var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResponse>>()
                ?? throw new InvalidOperationException(
                    $"No handler registered for query '{typeof(TQuery).Name}'. " +
                    $"Ensure an IQueryHandler<{typeof(TQuery).Name}, {typeof(TResponse).Name}> is registered in DI.");

            return handler.HandleAsync(query);
        }
    }
}
