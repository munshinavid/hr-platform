
using HRPlatform.ServiceBus.Abstractions;
using HRPlatform.Shared.Common;
using Microsoft.Extensions.Logging;

namespace Orchestrator.Handler.Common
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

        public async Task<TResponse> SendCommandAsync<TCommand, TResponse>(
            TCommand command)
            where TResponse : HandlerResult, new()
        {
            try
            {
                return await _serviceBus
                    .SendCommandAsync<TCommand, TResponse>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Service bus call failed for {CommandType}",
                    typeof(TCommand).Name);

                var result = new TResponse();
                result.Fail(Error.ServiceUnavailable(
                    "SERVICE_UNAVAILABLE",
                    "Target service is not available."));
                return result;
            }
        }

        public async Task<TResponse> SendQueryAsync<TQuery, TResponse>(
            TQuery query)
            where TResponse : HandlerResult, new()
        {
            try
            {
                return await _serviceBus
                    .SendQueryAsync<TQuery, TResponse>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Service bus call failed for {QueryType}",
                    typeof(TQuery).Name);

                var result = new TResponse();
                result.Fail(Error.ServiceUnavailable(
                    "SERVICE_UNAVAILABLE",
                    "Target service is not available."));

                return result;
            }
        }
    }
}