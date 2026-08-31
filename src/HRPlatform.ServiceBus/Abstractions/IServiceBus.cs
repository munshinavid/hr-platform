namespace HRPlatform.ServiceBus.Abstractions
{
    public interface IServiceBus
    {
        Task<TResult> SendCommandAsync<TCommand, TResult>(TCommand command);
        Task<TResult> SendQueryAsync<TQuery, TResult>(TQuery query);
    }
}
