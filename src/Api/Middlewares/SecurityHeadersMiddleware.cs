namespace Api.Middlewares;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        if (context.Request.Path.StartsWithSegments("/scalar"))
        {
            await _next(context);
            return;
        }

        var headers = context.Response.Headers;

        // 1. جلوگیری از MIME-type sniffing
        headers["X-Content-Type-Options"] = "nosniff";

        // 2. جلوگیری از Clickjacking
        headers["X-Frame-Options"] = "DENY"; // یا "SAMEORIGIN"

        // 3. XSS Protection (برای مرورگرهای قدیمی)
        headers["X-XSS-Protection"] = "1; mode=block";

        // 4. Referrer Policy
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // 5. Content Security Policy (CSP)
        headers["Content-Security-Policy"] =
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self'; " +
            "connect-src 'self' https://api.myapp.com; " +
            "frame-ancestors 'none';";

        // 6. Permissions Policy
        headers["Permissions-Policy"] =
            "camera=(), microphone=(), geolocation=(), " +
            "payment=(), usb=(), magnetometer=()";

        // 7. Strict Transport Security (HSTS)
        headers["Strict-Transport-Security"] =
            "max-age=31536000; includeSubDomains; preload";

        // 8. Cache Control
        headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
        headers["Pragma"] = "no-cache";

        // 9. Remove sensitive headers
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("X-AspNet-Version");
        context.Response.Headers.Remove("X-AspNetMvc-Version");

        await _next(context);
    }
}
