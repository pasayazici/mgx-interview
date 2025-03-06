using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace InterviewSystem.Infrastructure.Extensions;

public static class LoggingExtensions
{
    public static void LogUserAction(this ILogger logger, HttpContext context, string action)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        var userName = context.User.Identity?.Name ?? "anonymous";
        
        logger.LogInformation(
            "User Action: {Action} performed by User {UserName} (ID: {UserId})",
            action, userName, userId);
    }

    public static void LogAuthenticationAttempt(this ILogger logger, string username, bool success)
    {
        if (success)
        {
            logger.LogInformation("Successful authentication attempt for user: {Username}", username);
        }
        else
        {
            logger.LogWarning("Failed authentication attempt for user: {Username}", username);
        }
    }

    public static void LogAuthorizationFailure(this ILogger logger, HttpContext context, string resource)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        var userName = context.User.Identity?.Name ?? "anonymous";
        
        logger.LogWarning(
            "Authorization failure: User {UserName} (ID: {UserId}) attempted to access {Resource}",
            userName, userId, resource);
    }

    public static void LogSecurityEvent(this ILogger logger, string eventType, string details)
    {
        logger.LogInformation(
            "Security Event: {EventType} - {Details}",
            eventType, details);
    }

    public static void LogDatabaseOperation(this ILogger logger, string operation, string entity, bool success)
    {
        if (success)
        {
            logger.LogInformation(
                "Database Operation: {Operation} on {Entity} completed successfully",
                operation, entity);
        }
        else
        {
            logger.LogError(
                "Database Operation: {Operation} on {Entity} failed",
                operation, entity);
        }
    }

    public static void LogCriticalError(this ILogger logger, Exception exception, string operation)
    {
        logger.LogCritical(
            exception,
            "Critical error during {Operation}",
            operation);
    }

    public static void LogPerformanceMetric(this ILogger logger, string operation, long milliseconds)
    {
        logger.LogInformation(
            "Performance: {Operation} took {Milliseconds}ms to complete",
            operation, milliseconds);
    }

    public static IDisposable BeginNamedOperation(this ILogger logger, string operationName)
    {
        return logger.BeginScope(new Dictionary<string, object>
        {
            ["OperationName"] = operationName,
            ["StartTime"] = DateTime.UtcNow
        });
    }
}