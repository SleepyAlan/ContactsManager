using Entities;

namespace RepositoryContracts;

/// <summary>
/// Defines a repository contract for querying and persisting country entity.
/// </summary>
/// <remarks>Represents the abstraction used by the application layer to access country records independently of
/// the underlying data store.</remarks>
public interface ICountriesRepository
{
    /// <summary>
    /// Adds a country to the data store.
    /// </summary>
    /// <param name="country">The country to add.</param>
    /// <returns>A task that result is the country that was added.</returns>
    Task<Country> AddCountry(Country country);

    /// <summary>
    /// Updates a country in the data store. Ignores case sensitivity when comparing the country name.
    /// </summary>
    /// <param name="country">The country to update.</param>
    /// <returns>A task that result is the updated country.</returns>
    Task<Country> UpdateCountry(Country country);

    /// <summary>
    /// Deletes a country from the data store.
    /// </summary>
    /// <param name="countryId">The unique identifier of the country to delete.</param>
    /// <returns>A task that result is true if the country was deleted, or false if not found.</returns>
    Task<bool> DeleteCountry(Guid countryId);

    /// <summary>
    /// Gets a country by its unique identifier.
    /// </summary>
    /// <param name="countryId">The unique identifier of the country.</param>
    /// <returns>A task that result is the country with the specified identifier, or null if not found.</returns>
    Task<Country?> GetCountry(Guid countryId);

    /// <summary>
    /// Gets a country by its name.
    /// </summary>
    /// <param name="countryName">The name of the country.</param>
    /// <returns>A task that result is the country with the specified name, or null if not found.</returns>
    Task<Country?> GetCountry(string countryName);

    /// <summary>
    /// Gets all countries from the data store.
    /// </summary>
    /// <returns>A task that result is a list of all countries.</returns>
    Task<List<Country>> GetAllCountries();

    /// <summary>
    /// Checks if a country exists in the data store.
    /// </summary>
    /// <param name="countryId">The unique identifier of the country.</param>
    /// <returns>A task that result is true if the country exists, or false if not found.</returns>
    Task<bool> CountryExists(Guid countryId);

    /// <summary>
    /// Checks if a country exists in the data store. Ignores case sensitivity when comparing the country name.
    /// </summary>
    /// <param name="countryName">The name of the country.</param>
    /// <returns>A task that result is true if the country exists, or false if not found.</returns>
    Task<bool> CountryExists(string countryName);
}
