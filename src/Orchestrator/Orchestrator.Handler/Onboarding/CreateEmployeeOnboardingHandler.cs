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
        private readonly IServiceBus _serviceBus;
        private readonly ILogger<CreateEmployeeOnboardingHandler> _logger;

        public CreateEmployeeOnboardingHandler(
            IServiceBus serviceBus,
            ILogger<CreateEmployeeOnboardingHandler> logger)
        {
            _serviceBus = serviceBus;
            _logger     = logger;
        }

        public async Task<HandlerResult<CreateEmployeeOnboardingResponse>> HandleAsync(
            CreateEmployeeOnboardingCommand command)
        {
            var request = command.Request;

            _logger.LogInformation(
                "Onboarding: sending RegisterUserCommand for email {Email}", request.Email);

            var registerCommand = new RegisterUserCommand
            {
                Name     = request.Name,
                Email    = request.Email,
                Password = request.Password
            };

            HandlerResult<UserRegistrationResult> identityResult;
            try
            {
                identityResult = await _serviceBus
                    .SendCommandAsync<RegisterUserCommand, HandlerResult<UserRegistrationResult>>(
                        registerCommand);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex,
                    "Onboarding: no handler registered for RegisterUserCommand → HandlerResult<UserRegistrationResult>.");
                return HandlerResult<CreateEmployeeOnboardingResponse>.FailureResult(
                    "Identity service is not available.");
            }

            if (!identityResult.Success || identityResult.Data == null)
            {
                _logger.LogWarning(
                    "Onboarding: user creation failed for {Email}. Reason: {Reason}",
                    request.Email,
                    identityResult.Message);

                return HandlerResult<CreateEmployeeOnboardingResponse>.FailureResult(
                    $"User creation failed: {identityResult.Message}");
            }

            var userId = identityResult.Data.UserId;

            _logger.LogInformation(
                "Onboarding: user created with UserId={UserId}. Proceeding to create Employee.",
                userId);

            var createEmployeeCommand = new CreateEmployeeCommand
            {
                UserId         = userId,
                Name           = request.Name,
                Email          = request.Email,
                Phone          = request.Phone,
                Gender         = request.Gender,
                DepartmentId   = request.DepartmentId,
                JobTitle       = request.JobTitle,
                Salary         = request.Salary,
                EmploymentType = request.EmploymentType,
                JoiningDate    = request.JoiningDate,
                Status         = request.Status
            };

            HandlerResult<EmployeeResponse> employeeResult;
            try
            {
                employeeResult = await _serviceBus
                    .SendCommandAsync<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>(
                        createEmployeeCommand);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex,
                    "Onboarding: no handler registered for CreateEmployeeCommand.");
                
                await CompensateUserCreationAsync(userId);

                return HandlerResult<CreateEmployeeOnboardingResponse>.FailureResult(
                    "Employee service is not available. The newly created identity user was rolled back/deleted.");
            }

            if (employeeResult.Success && employeeResult.Data != null)
            {
                _logger.LogInformation(
                    "Onboarding: completed successfully. UserId={UserId}, EmployeeId={EmployeeId}",
                    userId,
                    employeeResult.Data.EmployeeId);

                return HandlerResult<CreateEmployeeOnboardingResponse>.SuccessResult(
                    new CreateEmployeeOnboardingResponse
                    {
                        UserId     = userId,
                        EmployeeId = employeeResult.Data.EmployeeId,
                        Name       = employeeResult.Data.Name,
                        Email      = employeeResult.Data.Email,
                        Message    = "Employee onboarding completed successfully."
                    },
                    "Employee onboarding completed successfully.");
            }

            _logger.LogError(
                "Onboarding: Employee creation failed after User creation. Triggering compensation for UserId={UserId}. Reason: {Reason}",
                userId,
                employeeResult.Message);

            await CompensateUserCreationAsync(userId);

            return HandlerResult<CreateEmployeeOnboardingResponse>.FailureResult(
                $"Employee creation failed: {employeeResult.Message}. The newly created identity user was rolled back/deleted.");
        }

        private async Task CompensateUserCreationAsync(int userId)
        {
            var deleteUserCommand = new DeleteUserCommand { UserId = userId };
            var compensationResult = await _serviceBus.SendCommandAsync<DeleteUserCommand, HandlerResult>(deleteUserCommand);
            
            if (compensationResult.Success)
            {
                _logger.LogInformation("Compensation successful: Deleted user with UserId={UserId}.", userId);
            }
            else
            {
                _logger.LogError("Compensation failed: Could not delete user with UserId={UserId}. Reason: {Reason}", userId, compensationResult.Message);
            }
        }
    }
}

