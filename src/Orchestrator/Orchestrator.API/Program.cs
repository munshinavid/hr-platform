using EmployeeManagement.Aggregator.Validation;
using EmployeeManagement.Handler;
using HRPlatform.ServiceBus.Extensions;
using HRPlatform.Shared.Extensions;
using HRPlatform.Shared.Middleware;
using IdentityManagement.Aggregator.Validation;
using IdentityManagement.Handler;
using Orchestrator.Handler;

var builder = WebApplication.CreateBuilder(args);

// ── Controllers 
builder.Services.AddControllers();

// ── Swagger / OpenAPI 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();


builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();


builder.Services.AddServiceBus();


builder.Services.AddIdentityHandlerLayer(builder.Configuration);

// ── Composition Root: EmployeeManagement subsystem 
builder.Services.AddEmployeeHandlerLayer(builder.Configuration);

// ── Orchestrator Handler layer 
builder.Services.AddOrchestratorHandlerLayer();

// ── Build 
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

