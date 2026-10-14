using Entities;
using ServiceContracts.DTOs;

namespace ServiceContracts.Extensions;

public static class CountryExtensions
{
    /// <summary>
    /// Converts a Country entity to a CountryResponse DTO.
    /// </summary>
    /// <param name="country">The Country entity to convert.</param>
    /// <returns>A CountryResponse DTO.</returns>
    public static CountryResponse ToCountryResponse(this Country country)
    {
        return new CountryResponse
        {
            CountryId = country.CountryId,
            CountryName = country.CountryName
        };
    }
}
