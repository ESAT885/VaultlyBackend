using Microsoft.AspNetCore.Mvc;
using VaultlyBackend.Api.Models.BaseModels;

namespace VaultlyBackend.Api.Extensions
{
    public static class ApiBehaviorExtensions
    {
        public static IServiceCollection AddCustomApiBehavior(
            this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var traceId = context.HttpContext.TraceIdentifier;

                    var errors = context.ModelState
                        .Where(x => x.Value!.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors
                                .Select(e => e.ErrorMessage)
                                .ToArray()
                        );

                    var response = ApiResponse<object>.Fail(
                        message: "Validation error",
                        errors: errors,
                        traceId: traceId
                    );

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}
