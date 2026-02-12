using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VaultlyBackend.Api.Models.BaseModels;

namespace VaultlyBackend.Api.Extensions
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = configuration["AppSettings:Issuer"],
                        ValidAudience = configuration["AppSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!)
                        )
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            var traceId = context.HttpContext.TraceIdentifier;

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var response = ApiResponse<object>.Fail(
                                "Yetkisiz erişim",
                                traceId: traceId
                            );

                            return context.Response.WriteAsJsonAsync(response);
                        },

                        OnForbidden = context =>
                        {
                            var traceId = context.HttpContext.TraceIdentifier;

                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var response = ApiResponse<object>.Fail(
                                "Bu işlem için yetkin yok",
                                traceId: traceId
                            );

                            return context.Response.WriteAsJsonAsync(response);
                        }
                    };
                });

            return services;
        }
    }
}
