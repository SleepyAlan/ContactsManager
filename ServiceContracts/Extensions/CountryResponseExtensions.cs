using Entities;
using ServiceContracts.DTOs;

namespace ServiceContracts.Extensions;

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
