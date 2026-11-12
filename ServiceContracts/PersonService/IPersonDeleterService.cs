using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using System.ComponentModel;

namespace ServiceContracts.PersonService;

/// <summary>
/// Represents a service contract for deleting persons.
/// </summary>
public interface IPersonDeleterService
{
    /// <summary>
    /// Deletes an existing person based on the provided personId.
    /// </summary>
    /// <param name="personId">The unique identifier of the person to delete.</param>
    /// <returns>A Task that results in true if the person was successfully deleted; otherwise, false.</returns>
    Task<bool> DeletePerson(Guid? personId);
}
