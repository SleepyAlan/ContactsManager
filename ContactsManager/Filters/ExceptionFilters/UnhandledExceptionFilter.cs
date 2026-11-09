using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Filters.ExceptionFilters;

public class UnhandledExceptionFilter : IExceptionFilter
{
    private readonly ILogger<UnhandledExceptionFilter> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    public UnhandledExceptionFilter(
        ILogger<UnhandledExceptionFilter> logger, 
        IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }
    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception,
            "An unhandled exception of type, {ExceptionType} occurred. Exception message: {ExceptionMessage}",
            context.Exception.GetType().Name,
            context.Exception.Message);

        if (_hostEnvironment.IsDevelopment())
        {
            context.Result = new ContentResult
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Content = $"An unexpected error occurred. Exception details: {context.Exception.Message}"
            };
        }
    }
}
