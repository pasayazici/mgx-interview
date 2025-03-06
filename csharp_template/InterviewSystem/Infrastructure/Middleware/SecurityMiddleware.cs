using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;

namespace InterviewSystem.Infrastructure.Middleware;

public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimiter _rateLimiter;

    public SecurityMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        
        var rateLimitConfig = configuration.GetSection("Security:RateLimiting").Get<RateLimitConfig>();
        _rateLimiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            PermitLimit = rateLimitConfig.PermitLimit,
            Window = TimeSpan.FromSeconds(rateLimitConfig.Window),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = rateLimitConfig.QueueLimit
        });
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add security headers
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("Content-Security-Policy", 
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self'; " +
            "connect-src 'self';");

        // Rate limiting
        using var lease = await _rateLimiter.AcquireAsync();
        if (!lease.IsAcquired)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return;
        }

        // Request validation
        if (!IsValidRequest(context.Request))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await _next(context);
    }

    private bool IsValidRequest(HttpRequest request)
    {
        // Validate content length
        if (request.ContentLength > 10_000_000) // 10MB limit
            return false;

        // Validate content type for POST/PUT requests
        if (request.Method is "POST" or "PUT")
        {
            var contentType = request.ContentType?.ToLower();
            if (string.IsNullOrEmpty(contentType) || 
                (!contentType.Contains("application/json") && 
                 !contentType.Contains("multipart/form-data")))
                return false;
        }

        return true;
    }
}

public class RateLimitConfig
{
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public int QueueLimit { get; set; }
}