# Custom Dispatcher / CQRS Abstraction

Handlers are currently injected **directly** as concrete types into `EmployeeController`.  
The goal is to introduce generic `ICommandHandler<,>` / `IQueryHandler<,>` interfaces, a lightweight `IDispatcher` contract, and a `Dispatcher` implementation that resolves handlers from the DI container at runtime — so the controller only sees `IDispatcher`.

No business logic, repository, domain/aggregator boundary, migration, or Unit-of-Work changes are made.

---

## Proposed Changes

### Handler Project — new abstractions

#### [NEW] `EmployeeManagement.Handler/Abstractions/ICommandHandler.cs`
```csharp
public interface ICommandHandler<TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}
```

#### [NEW] `EmployeeManagement.Handler/Abstractions/IQueryHandler.cs`
```csharp
public interface IQueryHandler<TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}
```

#### [NEW] `EmployeeManagement.Handler/Dispatcher/IDispatcher.cs`
```csharp
public interface IDispatcher
{
    Task<TResult> SendCommand<TCommand, TResult>(TCommand command, CancellationToken ct = default);
    Task<TResult> SendQuery<TQuery, TResult>(TQuery query, CancellationToken ct = default);
}
```

#### [NEW] `EmployeeManagement.Handler/Dispatcher/Dispatcher.cs`
Resolves `ICommandHandler<TCommand,TResult>` / `IQueryHandler<TQuery,TResult>` from `IServiceProvider` and invokes them.

---

### Existing handlers — implement the new interfaces (signature change only)

#### [MODIFY] `CreateEmployeeHandler.cs`
- Implement `ICommandHandler<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>`
- Add `CancellationToken ct = default` to `HandleAsync`

#### [MODIFY] `GetEmployeeHandler.cs`
- Implement `IQueryHandler<GetEmployeeQuery, HandlerResult<EmployeeResponse>>`
- Add `CancellationToken ct = default` to `HandleAsync`

---

### DI Registration — wire up the new types

#### [MODIFY] `EmployeeManagement.Handler/DependencyInjection.cs`
- Register `ICommandHandler<CreateEmployeeCommand, HandlerResult<EmployeeResponse>>` → `CreateEmployeeHandler`
- Register `IQueryHandler<GetEmployeeQuery, HandlerResult<EmployeeResponse>>` → `GetEmployeeHandler`
- Register `IDispatcher` → `Dispatcher` (Scoped)
- Remove direct concrete handler registrations

---

### API — controller uses IDispatcher only

#### [MODIFY] `EmployeeManagement.API/Controllers/EmployeeController.cs`
- Replace two concrete handler fields with `private readonly IDispatcher _dispatcher`
- Replace `_createEmployeeHandler.HandleAsync(command)` with `_dispatcher.SendCommand<...,... >(command)`
- Replace `_getEmployeeHandler.HandleAsync(query)` with `_dispatcher.SendQuery<...,... >(query)`

---

## Files changed (total: 6, all in Handler + API)

| Action | File |
|--------|------|
| NEW | `Handler/Abstractions/ICommandHandler.cs` |
| NEW | `Handler/Abstractions/IQueryHandler.cs` |
| NEW | `Handler/Dispatcher/IDispatcher.cs` |
| NEW | `Handler/Dispatcher/Dispatcher.cs` |
| MODIFY | `Handler/Commands/CreateEmployee/CreateEmployeeHandler.cs` |
| MODIFY | `Handler/Queries/GetEmployee/GetEmployeeHandler.cs` |
| MODIFY | `Handler/DependencyInjection.cs` |
| MODIFY | `API/Controllers/EmployeeController.cs` |

> **Not touched:** Repository, Aggregator, DTO, migrations, `Program.cs`, business logic, `HandlerResult`.

## Verification Plan
- `dotnet build EmployeeManagement.sln` — must compile with 0 errors
- Swagger POST `/api/Employee` and GET `/api/Employee/{id}` — same behavior as before


response:
Plan approved! Executing now.

Created task.md

**Step 1 — 4 টি নতুন abstraction file তৈরি করছি (parallel):**

Created ICommandHandler.cs
Created IQueryHandler.cs
Created IDispatcher.cs
Created Dispatcher.cs

**Step 2 — Existing handlers-এ interface implement করছি:**

Edited CreateEmployeeHandler.cs
Edited GetEmployeeHandler.cs

**Step 3 — DI Registration আপডেট করছি:**

Edited DependencyInjection.cs

**Step 4 — Controller refactor করছি:**

Edited EmployeeController.cs

**Step 5 — Build করে verify করছি:**

Ran command: `dotnet build EmployeeManagement.sln --no-incremental -v minimal 2>&1`
Used tool: manage_task
Used tool: manage_task

✅ **Build succeeded — 0 Errors, 0 Warnings.**

---

## সম্পন্ন! এখন project-এর structure:

```
Handler/
├── Abstractions/
│   ├── ICommandHandler.cs   ← NEW: generic command contract
│   └── IQueryHandler.cs     ← NEW: generic query contract
├── Dispatcher/
│   ├── IDispatcher.cs       ← NEW: dispatcher interface
│   └── Dispatcher.cs        ← NEW: DI-based runtime resolver
├── Commands/CreateEmployee/
│   ├── CreateEmployeeCommand.cs   (unchanged)
│   └── CreateEmployeeHandler.cs   ← implements ICommandHandler<,>
├── Queries/GetEmployee/
│   ├── GetEmployeeQuery.cs        (unchanged)
│   └── GetEmployeeHandler.cs      ← implements IQueryHandler<,>
└── DependencyInjection.cs         ← interface-mapped registrations
```

### কী পরিবর্তন হলো:

| Before | After |
|---|---|
| Controller-এ `CreateEmployeeHandler`, `GetEmployeeHandler` inject | শুধু `IDispatcher` inject |
| Concrete type registration: `AddScoped<CreateEmployeeHandler>()` | Interface mapping: `AddScoped<ICommandHandler<...>, CreateEmployeeHandler>()` |
| Handler-এ কোনো interface ছিল না | `ICommandHandler<,>` / `IQueryHandler<,>` implement করছে |

### নতুন Handler যোগ করতে এখন শুধু ৩টি কাজ:
1. `XxxCommand`/`XxxQuery` class বানাও
2. `XxxHandler : ICommandHandler<XxxCommand, TResult>` implement করো
3. `DependencyInjection.cs`-এ একটা `AddScoped<ICommandHandler<...>, XxxHandler>()` line যোগ করো — Controller ছুঁতে হবে না।