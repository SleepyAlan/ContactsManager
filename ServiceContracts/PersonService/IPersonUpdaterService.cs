using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using System.ComponentModel;

namespace ServiceContracts.PersonService;

/// <summary>
/// Represents a service contract for updating persons.
/// </summary>
public interface IPersonUpdaterService
{
    /// <summary>
    /// Updates an existing person based on the provided request.
    /// </summary>
    /// <param name="personUpdateRequest">The request containing the details of the person to update.</param>
    /// <returns>A Task that results in a PersonResponse dto containing the details of the updated person.</returns>
    Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
}
