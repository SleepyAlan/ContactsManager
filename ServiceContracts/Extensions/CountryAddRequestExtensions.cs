using Entities;
using ServiceContracts.DTOs;

namespace ServiceContracts.Extensions;

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