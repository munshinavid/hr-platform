
using IdentityManagement.Handler;
using HRPlatform.Shared.Extensions;
using HRPlatform.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger / OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

// Configure JWT Bearer authentication (validation only)
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



