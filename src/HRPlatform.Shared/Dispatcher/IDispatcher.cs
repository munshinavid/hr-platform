namespace HRPlatform.Shared.Dispatcher
{
    public interface IDispatcher
    {
        Task<TResponse> SendCommand<TCommand, TResponse>(TCommand command);

        Task<TResponse> SendQuery<TQuery, TResponse>(TQuery query);
    }
}
