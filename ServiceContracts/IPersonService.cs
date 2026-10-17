using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using System.ComponentModel;

namespace ServiceContracts;

/// <summary>
/// Represents a service contract for managing persons.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Adds a new person based on the provided request.
    /// </summary>
    /// <param name="personAddRequest">The request containing the details of the person to add.</param>
    /// <returns>PersonResponse dto containing the details of the added person.</returns>
    PersonResponse AddPerson(PersonAddRequest? personAddRequest);

    /// <summary>
    /// Updates an existing person based on the provided request.
    /// </summary>
    /// <param name="personUpdateRequest">The request containing the details of the person to update.</param>
    /// <returns>A PersonResponse dto containing the details of the updated person.</returns>
    PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    /// <summary>
    /// Deletes an existing person based on the provided personId.
    /// </summary>
    /// <param name="personId">The unique identifier of the person to delete.</param>
    /// <returns>True if the person was successfully deleted; otherwise, false.</returns>
    bool DeletePerson(Guid? personId);

    /// <summary>
    /// Retrieves all persons.
    /// </summary>
    /// <returns>A list of PersonResponse dto containing the details of all persons.</returns>
    List<PersonResponse> GetAllPersons();

    /// <summary>
    /// Retrieves a person by their unique identifier.
    /// </summary>
    /// <param name="personId">The unique identifier of the person.</param>
    /// <returns>A PersonResponse dto containing the details of the person, or null if not found.</returns>
    PersonResponse? GetPersonById(Guid? personId);

    /// <summary>
    /// Retrieves a list of persons based on the provided search string and search option.
    /// </summary>
    /// <param name="searchString">The search string to filter persons.</param>
    /// <param name="option">The search option to determine the filtering criteria.</param>
    /// <returns>A list of PersonResponse dto containing the details of the filtered persons.</returns>
    List<PersonResponse> GetFilteredPersons(string? searchString, SearchOptions option);

    /// <summary>
    /// Gets the provided persons sorted according to the specified search option and sort direction.
    /// </summary>
    /// <param name="personsToSort">Source list of PersonResponse objects to sort.</param>
    /// <param name="option">FilterOptions value that specifies which person property or criteria to use for ordering.</param>
    /// <param name="order">Sort direction — ascending or descending; defaults to Ascending.</param>
    /// <returns>A list of PersonResponse ordered by the specified option and direction.</returns>
    List<PersonResponse> GetSortedPersons(IEnumerable<PersonResponse> personsToSort, FilterOptions option, ListSortDirection order = ListSortDirection.Ascending);
}
