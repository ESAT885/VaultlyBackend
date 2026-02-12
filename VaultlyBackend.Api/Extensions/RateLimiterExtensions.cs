using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace VaultlyBackend.Api.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddRateLimiting(
           this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.GlobalLimiter =
                    PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    {
                        var ip = context.Connection.RemoteIpAddress?.ToString()
                                 ?? "unknown";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: ip,
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 100,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 10,
                                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                            });
                    });


                options.AddFixedWindowLimiter("login", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.QueueLimit = 2;
                });
            });

            return services;
        }
    }
}
