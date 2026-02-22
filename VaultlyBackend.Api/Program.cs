using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using VaultlyBackend.Api.Background;
using VaultlyBackend.Api.Data;
using VaultlyBackend.Api.Extensions;
using VaultlyBackend.Api.Helpers.Mapper;
using VaultlyBackend.Api.Middlewares;
using VaultlyBackend.Api.Services;
using VaultlyBackend.Api.Services.Interfaces;
using VaultlyBackend.Api.Strategies;
using VaultlyBackend.Api.Strategies.Interfaces;
using VaultlyBackend.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddRateLimiting();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
});
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddCustomApiBehavior();
builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:5178") // Vue dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Content-Disposition");
    });
});
builder.Services.AddDbContext<VaultlyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFfmpegVideoService, FfmpegVideoService>();
builder.Services.AddScoped<IVideoUploadService, VideoUploadService>();
builder.Services.AddScoped<IStoredFileService, StoredFileService>();
builder.Services.AddScoped<IFileProcessor, PdfFileProcessor>();
builder.Services.AddScoped<IFileProcessor, VideoFileProcessor>();
builder.Services.AddScoped<IFileProcessorFactory, FileProcessorFactory>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(_ =>
    new BackgroundTaskQueue(capacity: 100));

builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddMemoryCache();
var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseCors("AllowVue");
//app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
