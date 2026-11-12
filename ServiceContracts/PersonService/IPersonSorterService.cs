using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using System.ComponentModel;

namespace ServiceContracts.PersonService;

/// <summary>
/// Represents a service contract for sorting persons.
/// </summary>
public interface IPersonSorterService
{
    /// <summary>
    /// Gets the provided persons sorted according to the specified search option and sort direction.
    /// </summary>
    /// <param name="personsToSort">Source list of PersonResponse objects to sort.</param>
    /// <param name="option">FilterOptions value that specifies which person property or criteria to use for ordering.</param>
    /// <param name="order">Sort direction — ascending or descending; defaults to Ascending.</param>
    /// <returns>A list of PersonResponse ordered by the specified option and direction.</returns>
    List<PersonResponse> GetSortedPersons(IEnumerable<PersonResponse> personsToSort, FilterOptions option, ListSortDirection order = ListSortDirection.Ascending);
}
