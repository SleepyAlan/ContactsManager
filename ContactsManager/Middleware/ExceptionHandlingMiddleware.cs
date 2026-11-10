using Serilog;

namespace ContactsManager.Middleware;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IDiagnosticContext diagnosticContext)
    {
        _next = next;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.InnerException ?? ex, "An unhandled exception occurred.");

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            _diagnosticContext.Set("Exception", ex);

            var errorResponse = new
            {
                Message = "An unexpected error occurred. Please try again later."
            };

            await httpContext.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
