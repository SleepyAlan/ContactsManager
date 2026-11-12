using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using System.ComponentModel;

namespace ServiceContracts.PersonService;

/// <summary>
/// Represents a service contract for inserting a new person into the system.
/// </summary>
public interface IPersonAdderService
{
    /// <summary>
    /// Adds a new person based on the provided request.
    /// </summary>
    /// <param name="personAddRequest">The request containing the details of the person to add.</param>
    /// <returns>A Task that results in a PersonResponse dto containing the details of the added person.</returns>
    Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
}
