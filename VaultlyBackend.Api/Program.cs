using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VaultlyBackend.Api.Data;
using VaultlyBackend.Api.Extensions;
using VaultlyBackend.Api.Middlewares;
using VaultlyBackend.Api.Services;
using VaultlyBackend.Api.Services.Interfaces;
using VaultlyBackend.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRateLimiting();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddCustomApiBehavior();
builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<VaultlyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IAuthService, AuthService>();
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
