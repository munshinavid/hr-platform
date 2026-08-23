using EmployeeManagement.Handler;
using EmployeeManagement.Repository;
using EmployeeManagement.Shared.Extensions;
using EmployeeManagement.Shared.Middleware;
using EmployeeManagement.Handler.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationConfiguration(typeof(CreateEmployeeRequestValidator));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerConfiguration();

// Configure JWT Authentication
//builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddAuthorization();

builder.Services.AddRepositoryLayer(builder.Configuration);
builder.Services.AddHandlerLayer();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add authentication middleware BEFORE authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
