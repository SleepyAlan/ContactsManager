using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTOs;
using System.Reflection;

namespace ContactsManager.Filters.ActionFilters;

public class TrimWhiteSpaceFromRequestPropertiesFilter : IActionFilter
{
    private readonly ILogger<TrimWhiteSpaceFromRequestPropertiesFilter> _logger;

    public TrimWhiteSpaceFromRequestPropertiesFilter(ILogger<TrimWhiteSpaceFromRequestPropertiesFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var personAddRequest = context.ActionArguments.Values
            .FirstOrDefault(temp => temp is PersonAddRequest) as PersonAddRequest;
        if (personAddRequest != null)
        {
            SanitizeStringProperties(personAddRequest);
            return;
        }

        var personUpdateRequest = context.ActionArguments.Values
            .FirstOrDefault(temp => temp is PersonUpdateRequest) as PersonUpdateRequest;
        if (personUpdateRequest != null)
        {
            SanitizeStringProperties(personUpdateRequest);
            return;
        }

        var countryAddRequest = context.ActionArguments.Values
            .FirstOrDefault(temp => temp is CountryAddRequest) as CountryAddRequest;
        if (countryAddRequest != null)
        {
            SanitizeStringProperties(countryAddRequest);
            return;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private void SanitizeStringProperties(PersonAddRequest personRequest)
    {
        var properties = personRequest.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        SanitizeProperties(properties, personRequest);
    }

    private void SanitizeStringProperties(PersonUpdateRequest personRequest)
    {
        var properties = personRequest.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        SanitizeProperties(properties, personRequest);
    }

    private void SanitizeStringProperties(CountryAddRequest countryRequest)
    {
        var properties = countryRequest.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
        SanitizeProperties(properties, countryRequest);
    }

    private void SanitizeProperties(PropertyInfo[] properties, object request)
    {
        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string) && property.CanWrite)
            {
                var value = (string?)property.GetValue(request);
                if (value != null)
                {
                    property.SetValue(request, value.Trim());
                }
            }
        }

        _logger.LogInformation(
            "String properties sanitized for {RequestType}",
            request.GetType().Name);
    }
}