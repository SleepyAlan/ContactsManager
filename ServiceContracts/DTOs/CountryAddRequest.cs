using Entities;

namespace ServiceContracts.DTOs;

/// <summary>
/// Represents a DTO for inserting a new country.
/// </summary>
public class CountryAddRequest
{
    public string? CountryName { get; set; }
}

public static class CountryAddRequestExtensions
{
    public static Country ToCountry(this CountryAddRequest countryAddRequest)
    {
        return new Country
        {
            CountryName = countryAddRequest.CountryName
        };
    }
}