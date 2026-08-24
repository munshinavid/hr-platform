using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers
{
    /// <summary>
    /// Placeholder authentication controller.
    /// Future endpoint: POST /api/authentication/login
    /// JWT generation belongs here (Authentication subsystem), not in EmployeeManagement.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        // TODO: Inject IDispatcher and implement Login endpoint when
        // Authentication.Handler.Commands.Login.LoginHandler is implemented.
    }
}
