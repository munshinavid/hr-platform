# PROJECT_CONTEXT.md
> **Architectural context for AI agents.** Generated 2026-08-30 from a full repository scan.
> Read Section 1 first. Stop there if you only need a high-level mental model.

---

# 1. QUICK CONTEXT

## What This Project Is

**HRPlatform** is a .NET 8 HR management backend with two independently hosted REST APIs: one for authentication (register/login, JWT issuance) and one for employee CRUD. The target domain is internal HR operations â€” managing employees, departments, and user accounts. The project is in **active early development** with functional core features but incomplete authorization enforcement and no test suite.

## Business Domain

- **Authentication bounded context** â€” user registration, login, JWT token generation.
- **EmployeeManagement bounded context** â€” create, update, and query employees; each employee has an associated `User` account and belongs to a `Department`.

## Current Architecture

**Custom layered CQRS inside a modular structure.** Each bounded context is a physical folder containing five projects following the same layer naming convention:

```
[Context].API          <- HTTP entry point, thin controllers
[Context].Handler      <- use-case coordination (Commands + Queries)
[Context].Aggregator   <- domain entities, validation, mapping, domain rules
[Context].Repository   <- data access, EF Core, DbContext
[Context].DTO          <- request/response contracts (Commands, Queries, Responses)
```

Shared cross-cutting infrastructure lives in a single project: **`HRPlatform.Shared`**.

## Projects / Modules

| Project | Role |
|---|---|
| `Authentication.API` | Auth HTTP host (register, login) |
| `Authentication.Handler` | LoginHandler, RegisterUserHandler, JwtTokenService, PasswordHasher |
| `Authentication.Aggregator` | UserAggregatorRoot, validators, mapping |
| `Authentication.Repository` | AuthDbContext, AuthUserRepository |
| `Authentication.DTO` | LoginCommand, RegisterUserCommand, AuthResponse |
| `EmployeeManagement.API` | Employee HTTP host (CRUD + paged list) |
| `EmployeeManagement.Handler` | CreateEmployee, UpdateEmployee, GetEmployee, GetEmployees handlers |
| `EmployeeManagement.Aggregator` | EmployeeAggregatorRoot, UserAggregatorRoot, DepartmentAggregatorRoot, validators, mappers, DomainException |
| `EmployeeManagement.Repository` | EmployeeDbContext, GenericRepository, EmployeeRepository, UserRepository, DepartmentRepository, TransactionManager |
| `EmployeeManagement.DTO` | Commands, Queries, EmployeeResponse, PagedResponse |
| `HRPlatform.Shared` | IDispatcher, Dispatcher, ICommandHandler, IQueryHandler, HandlerResult, ApiErrorResponse, ExceptionHandlingMiddleware, JWT/Swagger/FluentValidation extensions |

## Dependency Direction

```
API -> Handler -> Aggregator
API -> DTO
API -> Shared
Handler -> Repository -> Aggregator
Handler -> DTO
Handler -> Shared
Repository -> Aggregator
Aggregator -> DTO           <- NOTE: Aggregator depends on DTO (established project convention)
Aggregator -> FluentValidation
DTO -> (nothing)
Shared -> (nothing domain-specific)
```

Aggregators depend on DTOs â€” this is an **established project convention, not a violation**.

## Main Request/Response Flow

```
HTTP Request
  -> Controller (thin: dispatches, checks result.Success, returns HTTP response)
    -> IDispatcher.SendCommand<TCommand, TResult>()
      -> ICommandHandler<TCommand, TResult>.HandleAsync()
        -> AggregatorRoot.MapToAggregator(command)   <- domain rules enforced here
        -> Repository.AddAsync() / UpdateAsync()      <- EF Core persistence
      -> HandlerResult<T> returned
    -> Controller maps to IActionResult
HTTP Response
```

Query flow uses `IDispatcher.SendQuery<TQuery, TResult>()` -> `IQueryHandler<TQuery, TResult>`.

## Architecture Rules (Non-Negotiable)

- **Controllers are thin** â€” dispatch and return HTTP results only.
- **Handlers own use-case logic** â€” orchestrate aggregators, repositories, and services.
- **Repositories handle all EF Core access** â€” never leak DbContext upward.
- **Aggregators own domain state and domain rules** â€” validation and mapping live here.
- **DTOs are contract objects** â€” no business logic.
- **Shared project is infrastructure-only** â€” no domain logic.
- **No MediatR** â€” a custom `IDispatcher` is already implemented and registered.
- **No EF Core in Aggregator or Handler** â€” EF stays in Repository only.
- **Authentication is JWT Bearer** â€” token generation in `Authentication.Handler`, validation via `HRPlatform.Shared.Extensions.JwtAuthenticationExtensions`.

## Current vs Future

| Current | Future (planned / not yet implemented) |
|---|---|
| Two separate .NET 8 API hosts | Possible merge or modular monolith |
| Custom Dispatcher (no MediatR) | â€” |
| CQRS at handler level | Full DDD event-driven CQRS |
| No automated tests | Test coverage |
| `[Authorize]` commented out | Full role-based authorization enforcement |
| Hardcoded default password in CreateEmployeeHandler | Proper default password management |

---

# 2. ARCHITECTURE MAP

## 2.1 Projects

### `Authentication.API`
- **Responsibility**: HTTP host for authentication endpoints.
- **Important folders**: `Controllers/`
- **Owns**: Routing, DI composition root, pipeline configuration.
- **Must not own**: Business logic, domain rules, DB access.

### `Authentication.Handler`
- **Responsibility**: CQRS command handlers for login and register; JWT generation service; password hashing service.
- **Important folders**: `Commands/Login/`, `Commands/Register/`, `Services/`
- **Owns**: Use-case orchestration, `IPasswordHasher`, `IJwtTokenService`, `JwtTokenService`.
- **Must not own**: EF Core, domain entity construction (delegates to Aggregator).

### `Authentication.Aggregator`
- **Responsibility**: Auth domain â€” `UserAggregatorRoot`, validators, mapper (`UserMapper`), `Roles` constants.
- **Important folders**: `Entities/`, `Validation/`, `Mapping/`, `Constants/`
- **Owns**: Domain state, input mapping, FluentValidation validators.
- **Must not own**: Repository, DbContext, HTTP concerns.
- **Depends on**: `Authentication.DTO` (project convention).

### `Authentication.Repository`
- **Responsibility**: Auth data access â€” `AuthDbContext`, `AuthUserRepository`.
- **Important folders**: `Data/`, `Implementations/`, `Interfaces/`
- **Depends on**: `Authentication.Aggregator` (returns domain entities from repository).

### `Authentication.DTO`
- **Responsibility**: Contract objects for auth use cases.
- **Owns**: `LoginCommand`, `RegisterUserCommand`, `AuthResponse`.
- **Must not own**: Logic or validation.

---

### `EmployeeManagement.API`
- **Responsibility**: HTTP host for employee endpoints.
- **Important folders**: `Controllers/`
- **Note**: `[Authorize]` and `[Authorize(Roles = "HR")]` are currently commented out.

### `EmployeeManagement.Handler`
- **Responsibility**: CQRS handlers for create, update, get (single), get (paged list).
- **Important folders**: `Commands/CreateEmployee/`, `Commands/UpdateEmployee/`, `Queries/GetEmployee/`, `Queries/GetEmployees/`, `Common/`
- **Owns**: Use-case orchestration, transaction management, local `HandlerResult` (see tech debt).

### `EmployeeManagement.Aggregator`
- **Responsibility**: EM domain â€” all three aggregator roots, validators, mappers, `DomainException`.
- **Important folders**: `Entities/`, `Validation/`, `Mapping/`, `Exception/`, `Constants/`
- **Depends on**: `EmployeeManagement.DTO` (project convention).

### `EmployeeManagement.Repository`
- **Responsibility**: EM data access â€” `EmployeeDbContext`, `GenericRepository<T>`, concrete repositories, `TransactionManager`.
- **Important folders**: `Data/`, `Implementations/`, `Interfaces/`
- **Depends on**: `EmployeeManagement.Aggregator`.

### `EmployeeManagement.DTO`
- **Responsibility**: Contract objects for employee use cases.
- **Owns**: `CreateEmployeeCommand`, `UpdateEmployeeCommand`, `GetEmployeeQuery`, `GetEmployeesQuery`, `EmployeeResponse`, `PagedResponse<T>`.

---

### `HRPlatform.Shared`
- **Responsibility**: Cross-cutting infrastructure shared by all bounded contexts.
- **Important folders**: `Abstractions/`, `Dispatcher/`, `Common/`, `Middleware/`, `Extensions/`
- **Must not own**: Domain entities, business rules, DB access.

---

## 2.2 Dependency Map

### Compile-Time Project References (verified from .csproj)

```
Authentication.API
  +-- Authentication.Handler
  |     +-- Authentication.Aggregator
  |     |     +-- Authentication.DTO
  |     +-- Authentication.DTO
  |     +-- Authentication.Repository
  |     |     +-- Authentication.Aggregator
  |     +-- HRPlatform.Shared
  +-- Authentication.DTO
  +-- HRPlatform.Shared

EmployeeManagement.API
  +-- EmployeeManagement.Handler
  |     +-- EmployeeManagement.Aggregator
  |     |     +-- EmployeeManagement.DTO
  |     +-- EmployeeManagement.DTO
  |     +-- EmployeeManagement.Repository
  |     |     +-- EmployeeManagement.Aggregator
  |     +-- HRPlatform.Shared
  +-- EmployeeManagement.Aggregator  [composition root / FluentValidation marker only]
  +-- EmployeeManagement.DTO
  +-- EmployeeManagement.Repository  [composition root / DI registration only]
  +-- HRPlatform.Shared
```

### Key Boundaries

- `Authentication` and `EmployeeManagement` share **no project references** to each other â€” isolated bounded contexts.
- Both contexts share the same SQL Server database (`EmployeeManagementDB`). The **Users table is shared** at the database level. `UserAggregatorRoot` is duplicated across both Aggregator projects with different field names (`PasswordHash` vs `Password`).
- `HRPlatform.Shared` has no references to any domain project.

---

## 2.3 Request / Use-Case Flow

### Command: Create Employee

```
POST /api/employee
  -> EmployeeController.Create([FromBody] CreateEmployeeCommand)
    -> _dispatcher.SendCommand<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>()
      -> CreateEmployeeHandler.HandleAsync(command)
          1. ITransactionManager.BeginTransactionAsync()
          2. BCrypt.HashPassword("Default@123")  [HARDCODED - tech debt]
          3. UserAggregatorRoot.MapToAggregator(command, hash, Roles.Employee)
          4. IUserRepository.AddAsync(user)         -> EF Core SaveChanges
          5. EmployeeAggregatorRoot.MapToAggregator(command, user.UserId)
               -> ValidateBusinessRules(salary, joiningDate) -- throws DomainException
          6. IEmployeeRepository.AddAsync(employee) -> EF Core SaveChanges
          7. ITransactionManager.CommitAsync()
          8. IEmployeeRepository.GetByIdAsync(employee.EmployeeId)  [reload with nav props]
          9. employee.MapToResponse() -> EmployeeResponse
         10. HandlerResult<EmployeeResponse>.SuccessResult(response, msg)
          catch DomainException -> Rollback -> HandlerResult.FailureResult
          catch Exception       -> Rollback -> HandlerResult.FailureResult + log
    -> Controller: result.Success -> 200 OK or 400 BadRequest(ApiErrorResponse)
```

### Command: Update Employee

```
PUT /api/employee/{employeeId}
  -> EmployeeController.Update([FromRoute] employeeId, [FromBody] UpdateEmployeeCommand)
    -> command.EmployeeId = employeeId  [set in controller before dispatch]
    -> UpdateEmployeeHandler.HandleAsync(command)
        1. BeginTransactionAsync()
        2. IEmployeeRepository.GetByIdAsync(command.EmployeeId)         -- null -> Rollback + fail
        3. IUserRepository.GetByIdAsync(employee.UserId)                -- null -> Rollback + fail
        4. IUserRepository.EmailExistsAsync(email, excludeUserId)       -- exists -> fail
        5. IEmployeeRepository.EmailExistsAsync(email, excludeEmployeeId) -- exists -> fail
        6. employee.MapToAggregator(command)  [validates + updates employee entity]
        7. user.MapToAggregator(command)      [updates user Name/Email]
        8. IUserRepository.UpdateAsync(user)
        9. IEmployeeRepository.UpdateAsync(employee)
       10. CommitAsync()
       11. reload + MapToResponse -> HandlerResult.SuccessResult
```

### Query: Get Employee by ID

```
GET /api/employee/{employeeId}
  -> GetEmployeeHandler.HandleAsync(GetEmployeeQuery { EmployeeId })
    -> IEmployeeRepository.GetByIdAsync(id)  [includes Department + User]
    -> EmployeeResponseMapper.MapToResponse(employee)
    -> HandlerResult<EmployeeResponse>.SuccessResult
  -> Controller: null -> 404 NotFound; else 200 OK
```

### Query: Get Employees (Paged)

```
GET /api/employee?pageNumber=1&pageSize=10
  -> GetEmployeesHandler.HandleAsync(GetEmployeesQuery)
    -> IEmployeeRepository.GetPagedAsync(pageNumber, pageSize)
         -> EF: .Include(Dept).Include(User).OrderBy(EmployeeId), CountAsync, Skip+Take
    -> employees.Select(EmployeeResponseMapper.MapToResponse)
    -> PagedResponse<EmployeeResponse> assembled
    -> HandlerResult<PagedResponse<EmployeeResponse>>.SuccessResult
```

### Command: Register User (Auth)

```
POST /api/authentication/register
  -> RegisterUserHandler.HandleAsync(RegisterUserCommand)
      1. IAuthUserRepository.EmailExistsAsync(email) -- exists -> fail
      2. IPasswordHasher.Hash(password)  [BCrypt]
      3. UserAggregatorRoot.MapToAggregator(command, hash, Roles.Employee)
      4. IAuthUserRepository.AddAsync(user)
      -> HandlerResult.SuccessResult / FailureResult
```

### Command: Login (Auth)

```
POST /api/authentication/login
  -> LoginHandler.HandleAsync(LoginCommand)
      1. IAuthUserRepository.GetByEmailAsync(email)               -- null -> fail (generic message)
      2. IPasswordHasher.Verify(password, user.PasswordHash)       -- false -> fail (generic message)
      3. IJwtTokenService.GenerateToken(user)
           -> HS256 JWT with claims: NameIdentifier=UserId, Email, Role
      4. AuthResponse { Token, TokenType="Bearer", ExpiresInMinutes }
      -> HandlerResult<AuthResponse>.SuccessResult
```

---

## 2.4 Domain Boundaries

### Authentication Bounded Context

| Object | Type | Purpose |
|---|---|---|
| `UserAggregatorRoot` (Auth) | Aggregate Root | UserId, Name, Email, PasswordHash, Role |
| `RegisterUserCommand` | DTO (mapping input) | Register request contract |
| `LoginCommand` | DTO | Login request contract |
| `AuthResponse` | Response DTO | JWT token delivery |

Auth aggregate MAY depend on `Authentication.DTO` (established convention). No domain validation â€” handled by FluentValidation validators.

### EmployeeManagement Bounded Context

| Object | Type | Purpose |
|---|---|---|
| `EmployeeAggregatorRoot` | Aggregate Root | Phone, Gender, DepartmentId, JobTitle, Salary, EmploymentType, JoiningDate, Status, UserId; nav: User, Department |
| `UserAggregatorRoot` (EM) | Sub-entity | Name, Email, Password, Role; nav: Employee |
| `DepartmentAggregatorRoot` | Lookup aggregate | DepartmentId, DepartmentName |
| `DomainException` | Domain exception | Thrown on rule violation, caught by handlers |

**EM domain rules enforced in `EmployeeAggregatorRoot.ValidateBusinessRules(salary, joiningDate)`:**
- Salary >= 0 (throws DomainException if negative)
- JoiningDate not in the future (throws DomainException)

---

## 2.5 CQRS / Handler Map

### Authentication Handlers

| Command | Handler | Repositories | Domain Behavior | Output |
|---|---|---|---|---|
| `RegisterUserCommand` | `RegisterUserHandler` | `IAuthUserRepository` | `UserAggregatorRoot.MapToAggregator()` | `HandlerResult` |
| `LoginCommand` | `LoginHandler` | `IAuthUserRepository` | None | `HandlerResult<AuthResponse>` |

### EmployeeManagement Handlers

| Command/Query | Handler | Repositories | Domain Behavior | Output |
|---|---|---|---|---|
| `CreateEmployeeCommand` | `CreateEmployeeHandler` | IEmployeeRepository, IUserRepository, ITransactionManager | UserAggregatorRoot.MapToAggregator + EmployeeAggregatorRoot.MapToAggregator + ValidateBusinessRules | `HandlerResult<EmployeeResponse>` |
| `UpdateEmployeeCommand` | `UpdateEmployeeHandler` | IEmployeeRepository, IUserRepository, ITransactionManager | employee.MapToAggregator + ValidateBusinessRules, user.MapToAggregator | `HandlerResult<EmployeeResponse>` |
| `GetEmployeeQuery` | `GetEmployeeHandler` | IEmployeeRepository | EmployeeResponseMapper.MapToResponse | `HandlerResult<EmployeeResponse>` |
| `GetEmployeesQuery` | `GetEmployeesHandler` | IEmployeeRepository | EmployeeResponseMapper.MapToResponse per item | `HandlerResult<PagedResponse<EmployeeResponse>>` |

> `IDepartmentRepository` is injected in `CreateEmployeeHandler` but **not called** â€” unused dead dependency.

---

## 2.6 Persistence Map

```
Handler
  | (via ITransactionManager for mutation handlers)
Repository (interface)
  |
GenericRepository<T> (FindAsync, ToListAsync, AddAsync+SaveChanges, UpdateAsync+SaveChanges)
  |
EmployeeDbContext / AuthDbContext
  |
SQL Server (EmployeeManagementDB) -- both contexts use the same database
```

### DbContexts

| Context | Entity Sets | Notes |
|---|---|---|
| `AuthDbContext` | `DbSet<UserAggregatorRoot> Users` | PasswordHash mapped to column "Password" |
| `EmployeeDbContext` | `DbSet<EmployeeAggregatorRoot> Employees`, `DbSet<DepartmentAggregatorRoot> Departments`, `DbSet<UserAggregatorRoot> Users` | 1:1 Employee-User (Cascade); Salary precision(18,2); seed data for 3 depts, 6 users, 5 employees |

### Transactions (EM only)
`ITransactionManager` / `TransactionManager` wraps `IDbContextTransaction` on `EmployeeDbContext`. Used in `CreateEmployeeHandler` and `UpdateEmployeeHandler`. Multiple `SaveChangesAsync()` calls within a transaction are valid â€” commit is explicit.

### IQueryable Exposure
`IEmployeeRepository.GetQueryable()` â€” declared and implemented (includes Dept + User, AsQueryable). Not called by any current handler.

---

## 2.7 Cross-Cutting Concerns

| Concern | Implementation | Notes |
|---|---|---|
| Dispatcher | `IDispatcher`/`Dispatcher` â€” resolves `ICommandHandler<,>` or `IQueryHandler<,>` from DI at runtime | HRPlatform.Shared; Scoped |
| FluentValidation | `AddFluentValidationAutoValidation()` + `AddValidatorsFromAssemblyContaining(markerType)` | Auto-400 before action; validators in Aggregator |
| Exception handling | `ExceptionHandlingMiddleware` â€” global catch, HTTP 500 + `ApiErrorResponse` JSON | HRPlatform.Shared |
| JWT validation | `JwtAuthenticationExtensions.AddJwtAuthentication(config)` â€” HS256 bearer | HRPlatform.Shared |
| JWT generation | `JwtTokenService.GenerateToken(user)` â€” HS256, claims NameIdentifier/Email/Role | Authentication.Handler |
| Swagger | `SwaggerExtensions.AddSwaggerConfiguration()` â€” with Bearer scheme | HRPlatform.Shared |
| Logging | `ILogger<T>` in handlers + middleware | ASP.NET Core built-in |
| DI registration | `AddXxxRepositoryLayer(config)` + `AddXxxHandlerLayer()` â€” called from each API Program.cs | Each bounded context |

---

# 3. DETAILED REFERENCE

## 3.1 File Inventory

### HRPlatform.Shared

| File | Types / Purpose |
|---|---|
| `Abstractions/ICommandHandler.cs` | `ICommandHandler<TCommand, TResult>` |
| `Abstractions/IQueryHandler.cs` | `IQueryHandler<TQuery, TResult>` |
| `Dispatcher/IDispatcher.cs` | `IDispatcher` â€” `SendCommand<,>`, `SendQuery<,>` |
| `Dispatcher/Dispatcher.cs` | `Dispatcher : IDispatcher` â€” resolves from `IServiceProvider` |
| `Common/HandlerResult.cs` | `HandlerResult`, `HandlerResult<T>` â€” static `SuccessResult`/`FailureResult` factories |
| `Common/ApiErrorResponse.cs` | `ApiErrorResponse { Message?, Details? }` |
| `Middleware/ExceptionHandlingMiddleware.cs` | Global catch; logs; returns 500 JSON |
| `Extensions/JwtAuthenticationExtensions.cs` | `AddJwtAuthentication(config)` |
| `Extensions/FluentValidationExtensions.cs` | `AddFluentValidationConfiguration(markerType)` |
| `Extensions/SwaggerExtensions.cs` | `AddSwaggerConfiguration()` with Bearer scheme |

### Authentication â€” Key Files

| File | Responsibility | Notes |
|---|---|---|
| `Authentication.API/Controllers/AuthenticationController.cs` | Register + Login endpoints | Thin; IDispatcher only |
| `Authentication.Handler/Commands/Login/LoginHandler.cs` | Login use case | IAuthUserRepository, IPasswordHasher, IJwtTokenService |
| `Authentication.Handler/Commands/Register/RegisterUserHandler.cs` | Register use case | IAuthUserRepository, IPasswordHasher |
| `Authentication.Handler/Services/JwtTokenService.cs` | JWT token generation | Reads IConfiguration; HS256 |
| `Authentication.Handler/DependencyInjection.cs` | Auth handler DI | Registers handlers, services, IDispatcher |
| `Authentication.Aggregator/Entities/UserAggregatorRoot.cs` | Auth user entity | Fields: UserId, Name, Email, PasswordHash, Role |
| `Authentication.Aggregator/Mapping/UserMapper.cs` | Maps RegisterUserCommand -> UserAggregatorRoot | Static |
| `Authentication.Aggregator/Validation/RegisterUserCommandValidator.cs` | Name/Email/Password rules | FluentValidation |
| `Authentication.Aggregator/Validation/LoginCommandValidator.cs` | Email/Password rules | FluentValidation |
| `Authentication.Aggregator/Constants/Roles.cs` | Role constants | HR, Employee |
| `Authentication.Repository/Data/AuthDbContext.cs` | EF Core context | Users DbSet; PasswordHash -> column "Password" |
| `Authentication.Repository/Implementations/AuthUserRepository.cs` | User data access | GetByEmail, EmailExists, AddAsync |
| `Authentication.Repository/DependencyInjection.cs` | Auth repository DI | |

### EmployeeManagement â€” Key Files

| File | Responsibility | Notes |
|---|---|---|
| `EmployeeManagement.API/Controllers/EmployeeController.cs` | CRUD + paged list endpoints | [Authorize] commented out |
| `EmployeeManagement.API/Program.cs` | DI composition root | JWT config commented out |
| `EmployeeManagement.Handler/Commands/CreateEmployee/CreateEmployeeHandler.cs` | Create use case | Transaction; hardcoded "Default@123" |
| `EmployeeManagement.Handler/Commands/UpdateEmployee/UpdateEmployeeHandler.cs` | Update use case | Transaction; dual email check |
| `EmployeeManagement.Handler/Queries/GetEmployee/GetEmployeeHandler.cs` | Get single employee | |
| `EmployeeManagement.Handler/Queries/GetEmployees/GetEmployeesHandler.cs` | Paged employee list | |
| `EmployeeManagement.Handler/Common/HandlerResult.cs` | **DUPLICATE** HandlerResult | Same structure as Shared; different CLR type â€” tech debt |
| `EmployeeManagement.Handler/DependencyInjection.cs` | EM handler DI | |
| `EmployeeManagement.Aggregator/Entities/EmployeeAggregatorRoot.cs` | Employee domain entity | ValidateBusinessRules, MapToAggregator, MapToResponse |
| `EmployeeManagement.Aggregator/Entities/UserAggregatorRoot.cs` | User sub-entity (EM copy) | Field: `Password` (not `PasswordHash`) |
| `EmployeeManagement.Aggregator/Entities/DepartmentAggregatorRoot.cs` | Department lookup | DepartmentId, DepartmentName only |
| `EmployeeManagement.Aggregator/Exception/DomainException.cs` | Domain rule violation | Caught by command handlers |
| `EmployeeManagement.Aggregator/Mapping/EmployeeMapper.cs` | Maps command -> EmployeeAggregatorRoot | Static; create + update variants |
| `EmployeeManagement.Aggregator/Mapping/EmployeeResponseMapper.cs` | Maps EmployeeAggregatorRoot -> EmployeeResponse | Accesses User + Department nav props |
| `EmployeeManagement.Aggregator/Mapping/UserMapper.cs` | Maps command -> UserAggregatorRoot (EM) | Uses `Password` field |
| `EmployeeManagement.Aggregator/Validation/CreateEmployeeCommandValidator.cs` | Full create validation | Gender/EmploymentType/Status enum checks |
| `EmployeeManagement.Aggregator/Validation/GetEmployeesQueryValidator.cs` | Pagination validation | PageNumber >= 1, PageSize > 0 |
| `EmployeeManagement.Aggregator/Constants/Roles.cs` | Role constants (EM) | Admin, HR, Employee |
| `EmployeeManagement.Repository/Data/EmployeeDbContext.cs` | EF Core context | All 3 entity sets; 1:1 Employee-User (Cascade); seed data |
| `EmployeeManagement.Repository/Implementations/GenericRepository.cs` | Base CRUD | GetByIdAsync uses FindAsync (NO includes) |
| `EmployeeManagement.Repository/Implementations/EmployeeRepository.cs` | Employee queries | Overrides GetAll+GetById with includes; GetPaged; EmailExists |
| `EmployeeManagement.Repository/Implementations/UserRepository.cs` | User queries | GetByEmail, EmailExists |
| `EmployeeManagement.Repository/Implementations/DepartmentRepository.cs` | Department | Thin â€” inherits GenericRepository only |
| `EmployeeManagement.Repository/Implementations/TransactionManager.cs` | DB transaction | BeginTransaction, Commit, Rollback on EmployeeDbContext |
| `EmployeeManagement.Repository/DependencyInjection.cs` | EM repository DI | |

---

## 3.2 Handlers

### `RegisterUserHandler`
- **Command**: `RegisterUserCommand`
- **Repositories**: `IAuthUserRepository` (EmailExistsAsync, AddAsync)
- **Domain**: `UserAggregatorRoot.MapToAggregator(command, hash, Roles.Employee)`
- **Output**: `HandlerResult` (no data payload)
- **Rules**: Email uniqueness in Auth Users table.
- **Transaction**: None â€” single SaveChanges in AddAsync.

### `LoginHandler`
- **Command**: `LoginCommand`
- **Repositories**: `IAuthUserRepository` (GetByEmailAsync)
- **Services**: `IPasswordHasher.Verify()`, `IJwtTokenService.GenerateToken()`
- **Output**: `HandlerResult<AuthResponse>`
- **Security**: Returns generic "Invalid email or password." for both missing user AND wrong password â€” prevents user enumeration.

### `CreateEmployeeHandler`
- **Command**: `CreateEmployeeCommand`
- **Repositories**: `IEmployeeRepository`, `IUserRepository`, `ITransactionManager`; `IDepartmentRepository` injected but **unused**.
- **Domain**: `UserAggregatorRoot.MapToAggregator()`, `EmployeeAggregatorRoot.MapToAggregator()` + `ValidateBusinessRules(salary, joiningDate)`
- **Transaction**: BeginTransaction -> AddUser -> AddEmployee -> Commit / Rollback on exception.
- **Known issue**: Hardcoded default password `"Default@123"` for every created employee.

### `UpdateEmployeeHandler`
- **Command**: `UpdateEmployeeCommand` (EmployeeId set from route in controller before dispatch)
- **Repositories**: `IEmployeeRepository`, `IUserRepository`, `ITransactionManager`
- **Domain**: `employee.MapToAggregator(command)` + `ValidateBusinessRules()`, `user.MapToAggregator(command)`
- **Email check**: BOTH `IUserRepository.EmailExistsAsync(email, excludeUserId)` AND `IEmployeeRepository.EmailExistsAsync(email, excludeEmployeeId)` â€” likely redundant.
- **Transaction**: BeginTransaction -> load -> validate -> UpdateUser -> UpdateEmployee -> Commit.

### `GetEmployeeHandler`
- **Query**: `GetEmployeeQuery`
- **Repositories**: `IEmployeeRepository.GetByIdAsync()` (includes User + Department)
- **Output**: `HandlerResult<EmployeeResponse>`; controller maps null to 404.

### `GetEmployeesHandler`
- **Query**: `GetEmployeesQuery` (PageNumber, PageSize)
- **Repositories**: `IEmployeeRepository.GetPagedAsync(pageNumber, pageSize)`
- **Output**: `HandlerResult<PagedResponse<EmployeeResponse>>`; ordered by EmployeeId ascending.

---

## 3.3 Repositories

### `IAuthUserRepository` / `AuthUserRepository`
- Context: `AuthDbContext` -> `Users` table
- Methods: `GetByEmailAsync`, `EmailExistsAsync`, `AddAsync`
- No business logic.

### `IGenericRepository<T>` / `GenericRepository<T>`
- Base CRUD: `GetByIdAsync` (FindAsync â€” NO navigation includes), `GetAllAsync`, `AddAsync`+SaveChanges, `UpdateAsync`+SaveChanges, `DeleteAsync`+SaveChanges
- Hardcoded to `EmployeeDbContext`.

### `IEmployeeRepository` / `EmployeeRepository`
- Extends `IGenericRepository<EmployeeAggregatorRoot>`
- **Overrides**: `GetAllAsync()` and `GetByIdAsync(id)` â€” both add `.Include(Department).Include(User)`.
- Extra: `GetByUserIdAsync(userId)`, `EmailExistsAsync(email, excludeEmployeeId)` (joins via Employee.User.Email), `GetPagedAsync(page, size)`, `GetQueryable()` (unused).
- No business logic.

### `IUserRepository` / `UserRepository`
- Extends `IGenericRepository<UserAggregatorRoot>`
- Extra: `GetByEmailAsync(email)`, `EmailExistsAsync(email, excludeUserId)`.

### `IDepartmentRepository` / `DepartmentRepository`
- Thin â€” inherits `GenericRepository<DepartmentAggregatorRoot>` with no extra methods.

### `ITransactionManager` / `TransactionManager`
- Wraps `IDbContextTransaction` on `EmployeeDbContext`.
- `BeginTransactionAsync()`, `CommitAsync()` (+dispose), `RollbackAsync()` (+dispose).

---

## 3.4 Aggregators / Domain

### `UserAggregatorRoot` (Authentication.Aggregator)

| Aspect | Detail |
|---|---|
| State | UserId, Name, Email, `PasswordHash`, Role |
| DB column mapping | `PasswordHash` -> column `"Password"` (EF config in AuthDbContext) |
| Methods | `MapToAggregator(RegisterUserCommand, hash, role)` static -> delegates to `UserMapper` |
| Domain rules | None â€” input validation via FluentValidation only |
| Dependencies | `Authentication.DTO` |

### `UserAggregatorRoot` (EmployeeManagement.Aggregator)

| Aspect | Detail |
|---|---|
| State | UserId, Name, Email, `Password`, Role; navigation: `Employee?` |
| DB column mapping | `Password` field name matches column name directly |
| Methods | `MapToAggregator(CreateEmployeeCommand, password, role)` static; `MapToAggregator(UpdateEmployeeCommand)` instance |
| Domain rules | None |
| Dependencies | `EmployeeManagement.DTO` |

> **CRITICAL**: Two separate `UserAggregatorRoot` classes with DIFFERENT field names (`PasswordHash` vs `Password`) both map to the same `Users` table in the same database. Schema changes affect both. Field name differences must be respected in EF config.

### `EmployeeAggregatorRoot`

| Aspect | Detail |
|---|---|
| State | EmployeeId, Phone, Gender, DepartmentId, JobTitle, Salary, EmploymentType, JoiningDate, Status, UserId; nav: `User`, `Department?` |
| Domain behavior | `ValidateBusinessRules(salary, joiningDate)` â€” throws `DomainException` (salary < 0 or joiningDate > UtcNow) |
| Methods | `MapToAggregator(CreateEmployeeCommand, userId)` static; `MapToAggregator(UpdateEmployeeCommand)` instance; `MapToResponse()` instance |
| Dependencies | `EmployeeManagement.DTO`, `DomainException`, mappers |

Domain behavior distinction:
- `ValidateBusinessRules()` = domain behavior (belongs in aggregate) CORRECT
- `MapToAggregator()` / `MapToResponse()` = mapping helpers (aggregate convention) CORRECT
- Persistence = repository only CORRECT

### `DepartmentAggregatorRoot`
- Lookup/reference data only. No behavior, no validation.
- Seeded with IT(1), HR(2), Finance(3).

### `DomainException`
- Simple `Exception` subclass. Thrown by `ValidateBusinessRules()`. Caught explicitly by command handlers, converted to `HandlerResult.FailureResult`.

---

## 3.5 DTOs

### Authentication

| DTO | Fields |
|---|---|
| `RegisterUserCommand` | Name, Email, Password |
| `LoginCommand` | Email, Password |
| `AuthResponse` | Token, TokenType="Bearer", ExpiresInMinutes |

### EmployeeManagement Commands

| DTO | Fields |
|---|---|
| `CreateEmployeeCommand` | Name, Email, Phone, Gender, DepartmentId, JobTitle, Salary, EmploymentType, JoiningDate, Status |
| `UpdateEmployeeCommand` | EmployeeId (route-injected by controller) + same fields as Create |

### EmployeeManagement Queries

| DTO | Fields |
|---|---|
| `GetEmployeeQuery` | EmployeeId |
| `GetEmployeesQuery` | PageNumber=1, PageSize=10 (defaults) |

### EmployeeManagement Responses

| DTO | Fields | Notes |
|---|---|---|
| `EmployeeResponse` | EmployeeId, Name, Email, Phone, Gender, DepartmentId, DepartmentName?, JobTitle, Salary, EmploymentType, JoiningDate, Status | Flattened: Name/Email from User, DepartmentName from Dept |
| `PagedResponse<T>` | Items, PageNumber, PageSize, TotalCount, TotalPages | Generic |

---

## 3.6 Validation

### A. Request/Input Validation (FluentValidation â€” auto-400, runs before controller action)

| Validator | Key Rules |
|---|---|
| `RegisterUserCommandValidator` | Name: NotEmpty, Length(2,100); Email: valid email; Password: NotEmpty, 6-20 chars |
| `LoginCommandValidator` | Email: valid email; Password: NotEmpty, min 6 chars |
| `CreateEmployeeCommandValidator` | Name, Email, Phone (11-digit regex), Gender enum (Male/Female/Other), DepartmentId > 0, Salary > 0, EmploymentType enum, JoiningDate, Status enum (Active/Inactive) |
| `GetEmployeesQueryValidator` | PageNumber >= 1, PageSize > 0 |

> `UpdateEmployeeCommandValidator.cs` exists on disk but is listed as `<None Include="...">` in the `.csproj` â€” **excluded from compilation**. No input validation runs on update requests.

### B. Database-Dependent Business Validation (Handler level)
- Email uniqueness on register: `IAuthUserRepository.EmailExistsAsync()`
- Email uniqueness on update: `IUserRepository.EmailExistsAsync()` AND `IEmployeeRepository.EmailExistsAsync()` â€” likely redundant
- Employee/User existence on update: null checks after `GetByIdAsync()`

### C. Domain Validation (Aggregator level â€” throws DomainException)
- Salary >= 0
- JoiningDate not in the future

---

## 3.7 Database Model

### Tables

| Table | PK | Notable Columns |
|---|---|---|
| `Users` | UserId (int) | Name, Email, Password (bcrypt hash), Role |
| `Departments` | DepartmentId (int) | DepartmentName; seeded: IT(1), HR(2), Finance(3) |
| `Employees` | EmployeeId (int) | Phone, Gender, DepartmentId (FK), JobTitle, Salary (decimal 18,2), EmploymentType, JoiningDate (DateTime), Status, UserId (FK, 1:1) |

### Relationships

| Relationship | Type | FK | Delete |
|---|---|---|---|
| Employee -> User | 1:1 (HasOne/WithOne) | Employee.UserId | Cascade |
| Employee -> Department | Many:1 via DepartmentId FK (not fully configured in EF) | Employee.DepartmentId | Not configured |

### Constraints
- `Users.Email` â€” no EF unique constraint; uniqueness enforced only at application level via `EmailExistsAsync`.
- `Employee.UserId` â€” 1:1 enforced by EF `HasOne().WithOne()`.
- `Salary` â€” precision (18, 2).
- Seeded: 3 departments, 6 users, 5 employee records.

---

## 3.8 Authentication / Authorization

### Mechanism
- JWT Bearer, HS256 symmetric key.
- Tokens issued by `Authentication.API` / `JwtTokenService`.
- Tokens validated by `JwtAuthenticationExtensions` (shared) using same key.

### Configuration Keys (structure only)
- `Jwt:SecretKey` â€” symmetric signing key
- `Jwt:Issuer` â€” token issuer
- `Jwt:Audience` â€” token audience
- `Jwt:ExpirationMinutes` â€” token lifetime

### Claims in JWT
| Claim | Value |
|---|---|
| `ClaimTypes.NameIdentifier` | user.UserId (string) |
| `ClaimTypes.Email` | user.Email |
| `ClaimTypes.Role` | user.Role ("HR" or "Employee") |

### Roles
- `"HR"`, `"Employee"` (both bounded contexts). `"Admin"` defined in EM Aggregator constants but never assigned.

### Authorization Status
- `EmployeeController`: `[Authorize]` and `[Authorize(Roles = "HR")]` commented out â€” all endpoints publicly accessible.
- `EmployeeManagement.API`: `AddJwtAuthentication()` commented out â€” tokens from Auth API cannot be validated by EM API.

### Middleware Pipeline Order
```
ExceptionHandlingMiddleware -> UseHttpsRedirection -> UseAuthentication -> UseAuthorization -> MapControllers
```

---

## 3.9 Architecture Decisions

**Decision**: Aggregators depend on DTOs.
**Why**: Reason not confirmed from source â€” likely pragmatic to avoid redundant intermediate objects.
**Current implication**: Aggregator project references DTO project. DTO field changes propagate to Aggregator.

---

**Decision**: Custom `IDispatcher` / `Dispatcher` instead of MediatR.
**Why**: Reason not confirmed from source.
**Current implication**: No pipeline behaviors. Handler registration is explicit in DI. Adding cross-cutting concerns requires modifying Dispatcher or wrapping handlers.

---

**Decision**: `HandlerResult<T>` duplicated in `EmployeeManagement.Handler.Common` and `HRPlatform.Shared.Common`.
**Why**: Reason not confirmed from source â€” likely an early structural inconsistency.
**Current implication**: EM handlers and EM controllers use the local copy. Structurally identical but different CLR types.

---

**Decision**: Single database for both bounded contexts.
**Why**: Reason not confirmed from source.
**Current implication**: `UserAggregatorRoot` physically duplicated with different field names. Schema coupling.

---

**Decision**: `UpdateEmployeeCommandValidator` excluded from compilation (`<None Include=...>`).
**Why**: Reason not confirmed from source â€” likely in active development.
**Current implication**: No FluentValidation on update requests.

---

**Decision**: Hardcoded `"Default@123"` in `CreateEmployeeHandler`.
**Why**: Reason not confirmed from source â€” development placeholder.
**Current implication**: All created employees share a known default password with no expiry or change mechanism.

---

## 3.10 Architecture Rules / Guardrails

**Rules Future AI Agents Must Follow**

1. **Controllers remain thin.** Dispatch -> check `result.Success` -> return `IActionResult`. No business logic.

2. **Handlers own use-case orchestration.** All coordination between repositories, domain objects, and services happens in handlers only.

3. **Repositories handle all EF Core access.** No handler or aggregator may reference `DbContext`, `DbSet`, or `Microsoft.EntityFrameworkCore`.

4. **Aggregators own domain state and business behavior.** `ValidateBusinessRules()` and `MapToAggregator()` live in aggregators. Do not move them.

5. **Aggregators may depend on DTOs.** Confirmed project convention â€” Aggregator.csproj references DTO.csproj.

6. **Aggregators must not depend on Repository, EF Core, DbContext, or HTTP.** Confirmed absent from both Aggregator .csproj files.

7. **Use the existing `IDispatcher` / `Dispatcher`.** Do not introduce MediatR or any alternative mediator.

8. **Do not bypass the dispatcher.** Controllers route all use cases through `IDispatcher`.

9. **Do not expose database entities through the API.** `EmployeeResponse` / `AuthResponse` are the API contracts. Aggregator roots are never serialized directly.

10. **Validators live in the Aggregator project.** FluentValidation validators are discovered by assembly scan. Do not place validators in Handler or API projects.

11. **Transactions only via `ITransactionManager`.** For multi-repository writes in EM, always use the transaction manager interface.

12. **Do not cross bounded context boundaries.** `Authentication.*` must not reference `EmployeeManagement.*` and vice versa.

13. **Do not silently change naming conventions.** `XxxAggregatorRoot`, `XxxHandler`, `MapToAggregator()`, `MapToResponse()`, `AddXxxRepositoryLayer()`, `AddXxxHandlerLayer()` are established patterns.

14. **Do not add abstractions without concrete need.** No speculative interfaces or generics.

15. **Respect the `<None Include>` exclusion of `UpdateEmployeeCommandValidator`.** Do not compile it without understanding the intent.

---

## 3.11 Current vs Future Architecture

### CURRENT

- Two independently hosted .NET 8 minimal API applications in one solution.
- Custom CQRS with `IDispatcher` (no MediatR, no pipeline behaviors).
- Layered project structure per bounded context: API | Handler | Aggregator | Repository | DTO.
- FluentValidation auto-validation registered from Aggregator assembly.
- EF Core 8 with SQL Server (EFCore.Tools present; migration files not found in scan).
- JWT Bearer auth â€” HS256, configured via `appsettings.json`.
- Global exception middleware.
- Swagger with Bearer scheme.
- BCrypt password hashing.
- Single shared SQL Server database (`EmployeeManagementDB`).
- No automated tests.
- Authorization enforcement commented out.

### PLANNED / FUTURE (confirmed from commented code and excluded files only)

- Role-based authorization: `[Authorize]`, `[Authorize(Roles = "HR")]` written and commented out â€” clearly planned.
- JWT validation in `EmployeeManagement.API`: `AddJwtAuthentication()` commented out â€” planned.
- `UpdateEmployeeCommandValidator` â€” file exists but excluded from build â€” in progress.
- `UserResponse` / `UserResponseMapper` â€” commented out in EM `UserAggregatorRoot` â€” deferred.

> No events, service bus, microservices, or other future architecture is confirmed from source. Do not describe as planned.

---

## 3.12 Technical Debt / Known Problems

### Critical

| Issue | Impact |
|---|---|
| Hardcoded default password `"Default@123"` in `CreateEmployeeHandler` | Security â€” all created employees share a known password; no expiry or change flow |
| Shared database + duplicate `UserAggregatorRoot` | Structural â€” schema changes risk breaking both bounded contexts; field name mismatch (`PasswordHash` vs `Password`) |

### High

| Issue | Impact |
|---|---|
| `[Authorize]` commented out on `EmployeeController` | Security â€” all employee endpoints publicly accessible |
| JWT validation disabled in `EmployeeManagement.API` | Security â€” Auth tokens cannot be validated by EM API |
| Duplicate email check in `UpdateEmployeeHandler` | Logic â€” same email checked twice via different join paths; likely redundant |
| `ExceptionHandlingMiddleware` exposes `exception.Message` in 500 response | Security â€” internal details leak to clients in production |

### Medium

| Issue | Impact |
|---|---|
| `HandlerResult` duplicated in Shared and EM.Handler.Common | Maintainability â€” structurally identical but different CLR types; divergence risk |
| `UpdateEmployeeCommandValidator` excluded from build | Quality â€” no input validation on update requests |
| `IDepartmentRepository` injected in `CreateEmployeeHandler` but unused | Dead dependency |
| `IEmployeeRepository.GetQueryable()` declared but never called | Dead code |
| No automated tests | Risk â€” all refactoring is unprotected |
| `SwaggerExtensions` title hardcoded as "Employee Management API" | Minor â€” incorrect for Auth API |

### Low

| Issue | Impact |
|---|---|
| `Roles` constants duplicated in both bounded contexts | Minor duplication |
| `Admin` role defined in EM constants but never assigned or enforced | Dead constant |
| No pagination filtering or sorting | Feature gap â€” `GetPagedAsync` orders by EmployeeId only |
| Response shape inconsistency | Anonymous types `{ message, employee }` from controllers; no unified envelope |

---

# 4. AI Decision-Making Guide

When proposing a refactor or new feature:

1. **Read QUICK CONTEXT first** (Section 1). Identify the bounded context and affected layer.
2. **Identify the affected project/layer.** API, Handler, Aggregator, Repository, DTO, or Shared?
3. **Check the dependency map** (Section 2.2). Ensure the change does not introduce a forbidden dependency direction.
4. **Check the relevant architecture rule** (Section 3.10). Verify compliance with guardrails.
5. **Inspect the relevant source files** before making code-level changes. This document reflects the state at generation time â€” verify current contents.
6. **Determine whether the proposed change follows the existing convention.** Pattern-match to existing handlers, repositories, and aggregators.
7. **Prefer the smallest maintainable change.** Do not restructure layers to fix a single bug.
8. **Avoid unnecessary abstractions.** No new interfaces or generics without concrete reuse need.
9. **Do not introduce frameworks without a concrete need.** No MediatR, no AutoMapper, no CQRS frameworks.
10. **Consider testability.** Inject dependencies as interfaces in handlers.
11. **Explain architectural trade-offs.** If a change deviates from convention, name the deviation and justify it.
12. **Never silently change an established convention.** Naming, DI patterns, and `HandlerResult` structure are conventions â€” flag changes explicitly.

**When multiple valid approaches exist:**
- Recommend the approach that **best fits the CURRENT project architecture** first.
- Label: `Current-convention compatible`
- If deviation required: `Requires architectural refactoring`
- If forward-looking: `Future-architecture oriented`
- Mention alternatives only when they materially affect the decision.

---

# 5. Source of Truth

- This document was generated after inspecting the **entire repository** (`HRPlatform.sln`).
- It is the **first architectural context** for future AI agents working on this project.
- It describes the **current implementation and project conventions** as of the generation date.
- AI agents must **verify relevant source files** before making code-level changes â€” this document may become stale.
- This document **must not invent architecture that does not exist**.

**Generation date**: 2026-08-30
**Solution**: `HRPlatform.sln`
**Target framework**: .NET 8
**Projects scanned**: 11 (Authentication: 5; EmployeeManagement: 5; HRPlatform.Shared: 1)

**Files inspected**: All .csproj files, all controllers, all handlers, all repositories and interfaces, all aggregator entities and mappers, all validators, both DbContexts, both Program.cs files, all DI registration files, all DTO classes, all Shared infrastructure files, both appsettings.json files.

**Important assumptions**:
- EF Core migration files were not found in the scanned directories. Database schema was inferred from EmployeeDbContext configuration and seed data. Migrations may exist in obj/ or excluded directories.
- No test projects exist in the solution.

**Unverified areas**:
- EmployeeManagement.API/appsettings.Development.json (127 bytes) â€” not fully read; unlikely to override meaningful configuration.
- Actual deployed migration state vs EF Core model.
