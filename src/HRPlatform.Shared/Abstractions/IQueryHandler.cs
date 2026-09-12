namespace HRPlatform.Shared.Abstractions
{
    public interface IQueryHandler<TQuery, TResponse>
    {
        Task<TResponse> HandleAsync(TQuery query);
    }
}
