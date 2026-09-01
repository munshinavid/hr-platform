using Orchestrator.DTO.Onboarding;

namespace Orchestrator.Handler.Onboarding
{
    /// <summary>
    /// Internal orchestration command — wraps the client-facing
    /// CreateEmployeeOnboardingRequest and carries it through the
    /// ICommandHandler pipeline within the Orchestrator context.
    /// </summary>
    public class CreateEmployeeOnboardingCommand
    {
        public CreateEmployeeOnboardingRequest Request { get; set; } = null!;
    }
}

