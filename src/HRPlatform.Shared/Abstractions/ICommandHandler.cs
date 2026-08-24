namespace HRPlatform.Shared.Abstractions
{
    public interface ICommandHandler<TCommand, TResult>
    {
        Task<TResult> HandleAsync(TCommand command);
    }
}
