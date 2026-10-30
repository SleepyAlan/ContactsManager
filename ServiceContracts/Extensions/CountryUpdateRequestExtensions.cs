using Entities;
using ServiceContracts.DTOs;

namespace ServiceContracts.Extensions;

public static class CountryUpdateRequestExtensions
{
    public static Country ToCountry(this CountryUpdateRequest? countryUpdateRequest)
    {
        if (countryUpdateRequest is null) return null!;
        return new Country
        {
            CountryId = countryUpdateRequest.CountryId,
            CountryName = countryUpdateRequest.CountryName
        };
    }
}