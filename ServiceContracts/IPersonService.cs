using ServiceContracts.DTOs;

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
    /// Retrieves all persons.
    /// </summary>
    /// <returns>A list of PersonResponse dto containing the details of all persons.</returns>
    List<PersonResponse> GetAllPersons();
}
