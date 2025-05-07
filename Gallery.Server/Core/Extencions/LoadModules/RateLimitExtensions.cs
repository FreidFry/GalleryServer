using System.Threading.RateLimiting;

namespace Gallery.Server.Core.Extencions.LoadModules
{
    internal static class RateLimitExtensions
    {
        internal static IServiceCollection AddRateLimitExtencions(this IServiceCollection services)
        {
            services.AddRateLimiter(option =>
            {
                option.AddPolicy("Global", httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 80,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 5,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 2,
                        }
                    )
                );

                option.AddPolicy("Login", httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(60),
                            SegmentsPerWindow = 6,
                            QueueLimit = 0,
                        }
                    )
                );
            });

            return services;
        }
    }
}
