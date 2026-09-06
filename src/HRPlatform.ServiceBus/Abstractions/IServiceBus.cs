namespace HRPlatform.ServiceBus.Abstractions
{
    public interface IServiceBus
    {
        Task<TResponse> SendCommandAsync<TCommand, TResponse>(TCommand command);
        Task<TResponse> SendQueryAsync<TQuery, TResponse>(TQuery query);
    }
}
