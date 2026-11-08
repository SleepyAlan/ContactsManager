using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace ContactsManager.Filters.ActionFilters;

public class PersonsListActionFilter : IActionFilter
{
    private Stopwatch? _stopwatch;
    private readonly ILogger<PersonsListActionFilter> _logger;

    public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _stopwatch?.Stop();
        _logger.LogInformation("Action executed in {ElapsedMilliseconds} ms", _stopwatch?.ElapsedMilliseconds);
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _stopwatch = Stopwatch.StartNew();
    }
}
