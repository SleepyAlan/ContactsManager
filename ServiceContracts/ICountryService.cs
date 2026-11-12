using ServiceContracts.CountryService;

namespace ServiceContracts;

/// <summary>
/// Represents the service contract for CRUD operations on Country entities.
/// </summary>
public interface ICountryService :
    ICountryAdderService,
    ICountryUpdaterService,
    ICountryDeleterService,
    ICountryGetterService
{
}
