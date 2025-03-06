using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace InterviewSystem.Infrastructure.Middleware;

public class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationMiddleware> _logger;

    public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Validate request content
            if (!await ValidateRequest(context.Request))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid request content" });
                return;
            }

            // Sanitize input parameters
            SanitizeQueryString(context.Request);
            if (context.Request.HasFormContentType)
            {
                SanitizeFormData(context.Request);
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in validation middleware");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "An internal server error occurred" });
        }
    }

    private async Task<bool> ValidateRequest(HttpRequest request)
    {
        // Content length validation
        if (request.ContentLength > 10_000_000) // 10MB limit
        {
            return false;
        }

        // Content type validation for POST/PUT
        if (request.Method is "POST" or "PUT")
        {
            var contentType = request.ContentType?.ToLower();
            if (string.IsNullOrEmpty(contentType) ||
                (!contentType.Contains("application/json") &&
                 !contentType.Contains("multipart/form-data") &&
                 !contentType.Contains("application/x-www-form-urlencoded")))
            {
                return false;
            }

            // Validate JSON content
            if (contentType.Contains("application/json"))
            {
                try
                {
                    using var reader = new StreamReader(request.Body);
                    var json = await reader.ReadToEndAsync();
                    request.Body.Position = 0; // Reset position for next middleware
                    
                    if (string.IsNullOrEmpty(json) || json.Length > 100_000) // 100KB limit for JSON
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void SanitizeQueryString(HttpRequest request)
    {
        var sanitizedQuery = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
        
        foreach (var (key, value) in request.Query)
        {
            var sanitizedKey = key.Sanitize();
            var sanitizedValues = value.Select(v => v.Sanitize()).ToArray();
            sanitizedQuery[sanitizedKey] = new Microsoft.Extensions.Primitives.StringValues(sanitizedValues);
        }

        request.Query = new QueryCollection(sanitizedQuery);
    }

    private void SanitizeFormData(HttpRequest request)
    {
        if (request.Form == null) return;

        var sanitizedForm = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
        
        foreach (var (key, value) in request.Form)
        {
            var sanitizedKey = key.Sanitize();
            var sanitizedValues = value.Select(v => v.Sanitize()).ToArray();
            sanitizedForm[sanitizedKey] = new Microsoft.Extensions.Primitives.StringValues(sanitizedValues);
        }

        request.Form = new FormCollection(sanitizedForm, request.Form.Files);
    }
}