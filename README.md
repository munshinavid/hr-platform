আগে দেখি তোমার `3-tier project` folder-এ কী আছে।

Listed directory 3-tier%20project%20updated
Listed directory employee-management-api
Listed directory EmployeeManagement.API
Listed directory EmployeeManagement.BLL
Listed directory EmployeeManagement.DAL
Listed directory Controllers
Listed directory Services
Listed directory Interfaces
Listed directory DTOs
Listed directory Repositories
Listed directory Extensions
Listed directory Auth
Listed directory Implementations
Listed directory Interfaces
Viewed AuthService.cs:1-171
Viewed AuthController.cs:1-58
Viewed AuthenticationExtensions.cs:1-49
Viewed LoginRequest.cs:1-11
Viewed LoginResultDto.cs:1-14
Viewed UserRepository.cs:1-22
Viewed RegisterRequest.cs:1-15

সব দেখলাম। এখন পুরো detailed guideline দিচ্ছি — কোন file কোথা থেকে আসবে, কোথায় বসবে, namespace কী হবে, extra কী add করতে হবে।

---

## 3-Tier → HR Platform Authentication Migration Guide

### Overview: কোন 3-tier layer কোন project-এ যাবে

```
3-tier                          →    HR Platform
──────────────────────────────────────────────────────
BLL/DTOs/Auth/                  →    Authentication.DTO/
BLL/Services/AuthService.cs     →    Authentication.Handler/Commands/Login/LoginHandler.cs
DAL/Repositories/UserRepository →    Authentication.Repository/Implementations/
API/Controllers/AuthController  →    Authentication.API/Controllers/AuthenticationController
API/Extensions/Authentication.. →    HRPlatform.Shared/Extensions/JwtAuthenticationExtensions.cs
```

---

## ① `Authentication.DTO` — DTOs বসাও

3-tier থেকে নাও: `BLL/DTOs/Auth/`

| 3-tier file | নতুন location | নতুন namespace |
|---|---|---|
| `LoginRequest.cs` | `Authentication.DTO/Commands/LoginCommand.cs` | `Authentication.DTO.Commands` |
| `LoginResultDto.cs` | `Authentication.DTO/Responses/AuthResponse.cs` | `Authentication.DTO.Responses` |
| `RegisterRequest.cs` | `Authentication.DTO/Commands/RegisterCommand.cs` | `Authentication.DTO.Commands` |

**`LoginCommand.cs`** — already exists, শুধু নিশ্চিত করো:
```csharp
namespace Authentication.DTO.Commands
{
    public class LoginCommand
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
```

**`AuthResponse.cs`** — 3-tier `LoginResultDto` থেকে নাও, rename করো:
```csharp
namespace Authentication.DTO.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Role { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
    }
}
```

**`RegisterCommand.cs`** — নতুন file, 3-tier `RegisterRequest` থেকে নাও:
```csharp
namespace Authentication.DTO.Commands
{
    public class RegisterCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee";
    }
}
```

---

## ② `Authentication.Repository` — UserRepository বসাও

3-tier থেকে নাও: `DAL/Repositories/Implementations/UserRepository.cs`

**কিন্তু সমস্যা হলো:** `Authentication.Repository` তে আলাদা DbContext নেই। `User` table আছে `EmployeeManagement.Repository` এর `EmployeeDbContext`-এ।

**Solution:** `Authentication.Repository.csproj` এ reference add করো:
```xml
<ProjectReference Include="..\..\EmployeeManagement\EmployeeManagement.Repository\EmployeeManagement.Repository.csproj" />
```

> ⚠️ এটা Repository layer reference — internal handler বা aggregator reference না। Architecture rule ভাঙছে না।

**`IAuthUserRepository.cs`** — already exists, শুধু update করো:
```csharp
namespace Authentication.Repository.Interfaces
{
    public interface IAuthUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> AddAsync(User user);           // Register এর জন্য
    }
}
```

*(এখানে `User` হলো `EmployeeManagement.Aggregator.Entities.User`)*

**`AuthUserRepository.cs`** — নতুন file, 3-tier `UserRepository` থেকে adapt করো:
```
Authentication.Repository/
└── Implementations/
    └── AuthUserRepository.cs
```

```csharp
using EmployeeManagement.Aggregator.Entities;
using EmployeeManagement.Repository.Data;        // EmployeeDbContext
using Microsoft.EntityFrameworkCore;
using Authentication.Repository.Interfaces;

namespace Authentication.Repository.Implementations
{
    public class AuthUserRepository : IAuthUserRepository
    {
        private readonly EmployeeDbContext _context;

        public AuthUserRepository(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
```

**`DependencyInjection.cs`** — update করো:
```csharp
services.AddScoped<IAuthUserRepository, AuthUserRepository>();
```
> DbContext আলাদা register করতে হবে না — EmployeeManagement.API ইতোমধ্যে `EmployeeDbContext` register করে।

---

## ③ `Authentication.Handler` — LoginHandler বসাও

3-tier থেকে নাও: `BLL/Services/AuthService.cs` → শুধু `LoginAsync()` + `GenerateJwtToken()` logic নাও

```
Authentication.Handler/
└── Commands/
    └── Login/
        └── LoginHandler.cs    ← NEW
    └── Register/
        └── RegisterHandler.cs ← NEW (optional, পরেও করা যাবে)
```

**`LoginHandler.cs`** — 3-tier `AuthService.LoginAsync` + `GenerateJwtToken` থেকে adapt:

```csharp
using Authentication.DTO.Commands;
using Authentication.DTO.Responses;
using Authentication.Handler.Common;
using Authentication.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Handler.Commands.Login
{
    public class LoginHandler
        : ICommandHandler<LoginCommand, HandlerResult<AuthResponse>>
    {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginHandler> _logger;

        // HandleAsync():
        //   1. GetByEmailAsync → null হলে FailureResult
        //   2. BCrypt.Verify → fail হলে FailureResult
        //   3. GenerateJwtToken() call করো
        //   4. SuccessResult(new AuthResponse { Token, Expiration, Role })
        
        // GenerateJwtToken():
        //   — 3-tier AuthService.GenerateJwtToken() হুবহু copy করো
        //   — JWT_SECRET env variable থেকে নাও (same pattern)
    }
}
```

**`HandlerResult.cs`** — Authentication.Handler এ নিজস্ব copy রাখো:
```
Authentication.Handler/
└── Common/
    └── HandlerResult.cs   ← EmployeeManagement.Handler.Common থেকে copy করো, namespace change করো
```
namespace: `Authentication.Handler.Common`

**`Authentication.Handler.csproj`** এ packages add করো:
```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.x.x" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
```

**`DependencyInjection.cs`** update করো:
```csharp
services.AddScoped<
    ICommandHandler<LoginCommand, HandlerResult<AuthResponse>>,
    LoginHandler>();

services.AddScoped<IDispatcher, Dispatcher>();
```

---

## ④ `HRPlatform.Shared` — JWT Extension বসাও

3-tier থেকে নাও: `API/Extensions/AuthenticationExtensions.cs`

```
HRPlatform.Shared/
└── Extensions/
    └── JwtAuthenticationExtensions.cs   ← NEW
```

3-tier থেকে হুবহু logic নাও, শুধু namespace পাল্টাও:
```csharp
namespace HRPlatform.Shared.Extensions
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 3-tier AuthenticationExtensions.cs এর পুরো code এখানে
            // JWT_SECRET env variable থেকে নাও — same pattern
        }
    }
}
```

> **Note:** `HRPlatform.Shared` তে `Microsoft.AspNetCore.Authentication.JwtBearer` package add করতে হবে।

---

## ⑤ `Authentication.API` — Controller + Program.cs update

3-tier থেকে নাও: `API/Controllers/AuthController.cs`

**`AuthenticationController.cs`** — 3-tier `AuthController` থেকে adapt, কিন্তু **IDispatcher use করো** (IAuthService না):

```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginCommand command)
{
    var result = await _dispatcher.SendCommand<LoginCommand, HandlerResult<AuthResponse>>(command);
    
    if (!result.Success)
        return Unauthorized(new ApiErrorResponse { Message = result.Message ?? "Unauthorized" });
    
    return Ok(result.Data);   // AuthResponse (Token, Expiration, Role)
}
```

**`Authentication.API.csproj`** এ add করো:
```xml
<ProjectReference Include="..\Authentication.Repository\Authentication.Repository.csproj" />
```

**`Program.cs`** এ uncomment/add করো:
```csharp
builder.Services.AddJwtAuthentication(builder.Configuration);  // HRPlatform.Shared থেকে
builder.Services.AddAuthRepositoryLayer();
builder.Services.AddAuthHandlerLayer();
```

**`appsettings.json`** এ add করো:
```json
"Jwt": {
    "Issuer": "HRPlatformApp",
    "Audience": "HRPlatformUsers",
    "ExpirationMinutes": 60
}
```
> `JWT_SECRET` — environment variable হিসেবে রাখো (`.env` বা system env), appsettings এ রাখবে না। 3-tier project এ `.env` file আছে, same pattern follow করো।

---

## Summary Table — কোথা থেকে, কোথায়

| 3-tier source | HR Platform destination | Action |
|---|---|---|
| `BLL/DTOs/Auth/LoginRequest.cs` | `Authentication.DTO/Commands/LoginCommand.cs` | Adapt + namespace change |
| `BLL/DTOs/Auth/LoginResultDto.cs` | `Authentication.DTO/Responses/AuthResponse.cs` | Adapt + namespace change |
| `BLL/DTOs/Auth/RegisterRequest.cs` | `Authentication.DTO/Commands/RegisterCommand.cs` | Adapt + namespace change |
| `DAL/Repositories/.../UserRepository.cs` | `Authentication.Repository/Implementations/AuthUserRepository.cs` | Adapt (EmployeeDbContext use করবে) |
| `BLL/Services/AuthService.cs` (LoginAsync + GenerateJwtToken) | `Authentication.Handler/Commands/Login/LoginHandler.cs` | Adapt to ICommandHandler pattern |
| `BLL/Services/AuthService.cs` (RegisterAsync) | `Authentication.Handler/Commands/Register/RegisterHandler.cs` | Optional, same pattern |
| `API/Controllers/AuthController.cs` | `Authentication.API/Controllers/AuthenticationController.cs` | Adapt — IDispatcher use করো |
| `API/Extensions/AuthenticationExtensions.cs` | `HRPlatform.Shared/Extensions/JwtAuthenticationExtensions.cs` | Haubhu copy, namespace change |
| `BLL/Common/HandlerResult` equivalent | `Authentication.Handler/Common/HandlerResult.cs` | EM.Handler থেকে copy, namespace change |

---

## Extra যা add করতে হবে (3-tier তে নেই)

1. **`Authentication.Handler/Common/HandlerResult.cs`** — `EmployeeManagement.Handler.Common.HandlerResult` থেকে copy, namespace → `Authentication.Handler.Common`
2. **`Authentication.Repository.csproj`** → `EmployeeManagement.Repository` reference
3. **`Authentication.Handler.csproj`** → BCrypt, JWT packages
4. **`HRPlatform.Shared`** → JWT package + `JwtAuthenticationExtensions.cs`
5. **`EmployeeManagement.API/Program.cs`** → `AddJwtAuthentication()` uncomment (shared extension use করবে)