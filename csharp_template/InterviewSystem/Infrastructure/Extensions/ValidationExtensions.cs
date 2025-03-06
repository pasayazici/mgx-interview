using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace InterviewSystem.Infrastructure.Extensions;

public static class ValidationExtensions
{
    private static readonly Regex HtmlTagRegex = new("<.*?>", RegexOptions.Compiled);
    private static readonly Regex ScriptRegex = new(@"<script[^>]*>[\s\S]*?</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex SqlInjectionRegex = new(@"(\b(select|insert|update|delete|drop|union|exec|declare)\b)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex XssRegex = new(@"((javascript|vbscript|expression|applet|meta|xml|blink|link|style|script|embed|object|iframe|frame|frameset|ilayer|layer|bgsound|title|base|onload|onunload|onchange|onsubmit|onreset|onselect|onblur|onfocus|onabort|onkeydown|onkeypress|onkeyup|onclick|ondblclick|onmousedown|onmousemove|onmouseout|onmouseover|onmouseup|oncut|oncopy|onpaste))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string Sanitize(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // HTML encode the input
        var encoded = HttpUtility.HtmlEncode(input);

        // Remove HTML tags
        encoded = HtmlTagRegex.Replace(encoded, string.Empty);

        // Remove script tags and content
        encoded = ScriptRegex.Replace(encoded, string.Empty);

        // Replace potential SQL injection patterns
        encoded = SqlInjectionRegex.Replace(encoded, string.Empty);

        // Remove potential XSS patterns
        encoded = XssRegex.Replace(encoded, string.Empty);

        // Trim whitespace
        encoded = encoded.Trim();

        return encoded;
    }

    public static bool ContainsSqlInjection(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        return SqlInjectionRegex.IsMatch(input);
    }

    public static bool ContainsXss(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        return XssRegex.IsMatch(input);
    }

    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidPhoneNumber(this string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return false;

        // Basic phone number validation (can be customized based on requirements)
        return Regex.IsMatch(phoneNumber, @"^\+?[\d\s-]{10,}$");
    }

    public static bool IsStrongPassword(this string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        // Minimum 12 characters, at least one uppercase letter, one lowercase letter,
        // one number and one special character
        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$");
        return regex.IsMatch(password);
    }

    public static bool IsValidDateFormat(this string date)
    {
        return DateTime.TryParse(date, out _);
    }

    public static bool IsValidGuid(this string guid)
    {
        return Guid.TryParse(guid, out _);
    }
}