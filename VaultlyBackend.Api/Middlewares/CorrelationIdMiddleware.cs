namespace VaultlyBackend.Api.Middlewares
{
    public class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var id)
            ? id.ToString()
            : Guid.NewGuid().ToString();

            // HttpContext içine ekle
            context.Items[HeaderName] = correlationId;

            // Response header olarak geri dön
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
