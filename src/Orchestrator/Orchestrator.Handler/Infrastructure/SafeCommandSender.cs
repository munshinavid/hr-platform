
using HRPlatform.ServiceBus.Abstractions;
using HRPlatform.Shared.Common;
using Microsoft.Extensions.Logging;

namespace Orchestrator.Handler.Infrastructure
{
    public class SafeCommandSender
    {
        private readonly IServiceBus _serviceBus;
        private readonly ILogger<SafeCommandSender> _logger;

        public SafeCommandSender(
            IServiceBus serviceBus,
            ILogger<SafeCommandSender> logger)
        {
            _serviceBus = serviceBus;
            _logger = logger;
        }

        public async Task<TResult> SendCommandAsync<TCommand, TResult>(
            TCommand command)
            where TResult : HandlerResult, new()
        {
            try
            {
                return await _serviceBus
                    .SendCommandAsync<TCommand, TResult>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Service bus call failed for {CommandType}",
                    typeof(TCommand).Name);

                var result = new TResult();
                result.Fail(Error.ServiceUnavailable(
                    "SERVICE_UNAVAILABLE",
                    "Target service is not available."));
                return result;
            }
        }

        public async Task<TResult> SendQueryAsync<TQuery, TResult>(
            TQuery query)
            where TResult : HandlerResult, new()
        {
            try
            {
                return await _serviceBus
                    .SendQueryAsync<TQuery, TResult>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Service bus call failed for {QueryType}",
                    typeof(TQuery).Name);

                var result = new TResult();
                result.Fail(Error.ServiceUnavailable(
                    "SERVICE_UNAVAILABLE",
                    "Target service is not available."));

                return result;
            }
        }
    }
}