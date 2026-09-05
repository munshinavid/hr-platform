using EmployeeManagement.DTO.Command;
using EmployeeManagement.DTO.Response;
using HRPlatform.ServiceBus.Abstractions;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using IdentityManagement.DTO.Command;
using IdentityManagement.DTO.Response;
using Microsoft.Extensions.Logging;
using Orchestrator.DTO.Onboarding;

namespace Orchestrator.Handler.Onboarding
{
    public class CreateEmployeeOnboardingHandler
        : ICommandHandler<CreateEmployeeOnboardingCommand, HandlerResult<CreateEmployeeOnboardingResponse>>
    {
        private readonly Infrastructure.SafeCommandSender _safeCommandSender;
        private readonly ILogger<CreateEmployeeOnboardingHandler> _logger;

        public CreateEmployeeOnboardingHandler(
            Infrastructure.SafeCommandSender safeCommandSender,
            ILogger<CreateEmployeeOnboardingHandler> logger)
        {
            _safeCommandSender = safeCommandSender;
            _logger     = logger;
        }

        public async Task<HandlerResult<CreateEmployeeOnboardingResponse>> HandleAsync(
            CreateEmployeeOnboardingCommand command)
        {
            var registerCommand = OnboardingCommandMapper.ToRegisterUserCommand(command);

            var identityResult = await _safeCommandSender
                .SendCommandAsync<RegisterUserCommand, HandlerResult<UserRegistrationResult>>(registerCommand);

            if (!identityResult.Success || identityResult.Data == null)
            {
                return HandlerResult<CreateEmployeeOnboardingResponse>.FailureResult(identityResult.Error);
            }

            var userId = identityResult.Data.UserId;

            var createEmployeeCommand = OnboardingCommandMapper.ToCreateEmployeeCommand(command, userId);

            var employeeResult = await _safeCommandSender
                .SendCommandAsync<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>(createEmployeeCommand);

            if (employeeResult.Success && employeeResult.Data != null)
            {
                return HandlerResult<CreateEmployeeOnboardingResponse>.SuccessResult(
                    new CreateEmployeeOnboardingResponse
                    {
                        UserId     = userId,
                        EmployeeId = employeeResult.Data.EmployeeId,
                        Name       = employeeResult.Data.Name,
                        Email      = employeeResult.Data.Email
                    },
                    "Employee onboarding completed successfully.");
            }

            _logger.LogError(
                "Onboarding failed after User creation. Triggering compensation for UserId={UserId}.",
                userId);

            await CompensateUserCreationAsync(userId);

            return HandlerResult<CreateEmployeeOnboardingResponse>.FailureResult(employeeResult.Error);
        }

        private async Task CompensateUserCreationAsync(int userId)
        {
            var deleteUserCommand = new DeleteUserCommand { UserId = userId };
            var compensationResult = await _safeCommandSender.SendCommandAsync<DeleteUserCommand, HandlerResult>(deleteUserCommand);
            
            if (!compensationResult.Success)
            {
                _logger.LogCritical("Compensation failed for UserId={UserId}.", userId);
            }
        }
    }
}

