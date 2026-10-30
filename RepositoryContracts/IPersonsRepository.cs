using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts;

/// <summary>
/// Defines a repository contract for querying and persisting person entity.
/// </summary>
/// <remarks>Represents the abstraction used by the application layer to access person records independently of
/// the underlying data store.</remarks>
public interface IPersonsRepository
{
    /// <summary>
    /// Adds a person to the data store.
    /// </summary>
    /// <param name="person">The person to add.</param>
    /// <returns>A task that result is the person that was added.</returns>
    Task<Person> AddPerson(Person person);

    /// <summary>
    /// Updates a person in the data store. Ignores case sensitivity when comparing the person name.
    /// </summary>
    /// <param name="person">The person to update.</param>
    /// <returns>A task that result is the updated person. Returns null if the person was not found.</returns>
    Task<Person?> UpdatePerson(Person person);

    /// <summary>
    /// Deletes a person from the data store.
    /// </summary>
    /// <param name="personId">The unique identifier of the person to delete.</param>
    /// <returns>A task that result is true if the person was deleted, or false if not found.</returns>
    Task<bool> DeletePerson(Guid personId);

    /// <summary>
    /// Gets a person by its unique identifier.
    /// </summary>
    /// <param name="personId">The unique identifier of the person.</param>
    /// <returns>A task that result is the person with the specified identifier, or null if not found.</returns>
    Task<Person?> GetPerson(Guid personId);

    /// <summary>
    /// Gets all persons from the data store.
    /// </summary>
    /// <returns>A task that result is a list of all persons.</returns>
    Task<List<Person>> GetAllPersons();

    /// <summary>
    /// Checks if a person exists in the data store.
    /// </summary>
    /// <param name="personId">The unique identifier of the person.</param>
    /// <returns>A task that result is true if the person exists, or false if not found.</returns>
    Task<bool> PersonExists(Guid personId);

    /// <summary>
    /// Gets all persons from the data store that belong to a specific country.
    /// </summary>
    /// <param name="countryId">The unique identifier of the country.</param>
    /// <returns>A task that result is a list of persons belonging to the specified country.</returns>
    Task<List<Person>> GetPersonsByCountry(Guid countryId);

    /// <summary>
    /// Gets all persons from the data store that belong to a specific country.
    /// </summary>
    /// <param name="countryName">The name of the country.</param>
    /// <returns>A task that result is a list of persons belonging to the specified country.</returns>
    Task<List<Person>> GetPersonsByCountry(string countryName);
    
    /// <summary>
    /// Gets all persons from the data store that belong to a specific country.
    /// </summary>
    /// <param name="country">The country entity.</param>
    /// <returns>A task that result is a list of persons belonging to the specified country.</returns>
    Task<List<Person>> GetPersonsByCountry(Country country);

    /// <summary>
    /// Gets all persons from the data store that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The expression to filter persons.</param>
    /// <returns>A task that result is a list of persons that match the specified predicate.</returns>
    Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);
}
