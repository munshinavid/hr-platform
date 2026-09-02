using IdentityManagement.Aggregator.Validation;
using IdentityManagement.Handler;
using HRPlatform.Shared.Extensions;
using HRPlatform.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// FluentValidation — scans IdentityManagement.Aggregator assembly for all validators
builder.Services.AddFluentValidationConfiguration(typeof(RegisterUserCommandValidator));

// Learn more about configuring Swagger/OpenAPI 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

// Configure JWT Bearer authentication (validation only — token generation is in IdentityManagement.Handler)
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

// Handler layer (mirrors EM.API Program.cs convention)
builder.Services.AddIdentityHandlerLayer(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication must come before Authorization in the pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



