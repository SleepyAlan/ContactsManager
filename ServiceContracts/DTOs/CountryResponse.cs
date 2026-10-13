using Entities;

namespace ServiceContracts.DTOs;

/// <summary>
/// Represents the response DTO for Country entity.
/// It is a return type of most CountriesService methods.
/// </summary>
public class CountryResponse
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }

}

public static class CountryResponseExtensions
{
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse
        {
            CountryId = country.CountryId,
            CountryName = country.CountryName
        };
    }


}
