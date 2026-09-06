namespace HRPlatform.Shared.Abstractions
{
    public interface ICommandHandler<TCommand, TResponse>
    {
        Task<TResponse> HandleAsync(TCommand command);
    }
}
