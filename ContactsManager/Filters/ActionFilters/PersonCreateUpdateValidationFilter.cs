using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTOs;
using System.Reflection;

namespace ContactsManager.Filters.ActionFilters;

public class PersonCreateUpdateValidationFilter : IActionFilter
{
    private readonly ILogger<PersonCreateUpdateValidationFilter> _logger;

    public PersonCreateUpdateValidationFilter(ILogger<PersonCreateUpdateValidationFilter> logger)
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

    private void SanitizeProperties(PropertyInfo[] properties, object personRequest)
    {
        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(string) && property.CanWrite)
            {
                var value = (string?)property.GetValue(personRequest);
                if (value != null)
                {
                    property.SetValue(personRequest, value.Trim());
                }
            }
        }

        _logger.LogInformation(
            "String properties sanitized for {RequestType}",
            personRequest.GetType().Name);
    }
}