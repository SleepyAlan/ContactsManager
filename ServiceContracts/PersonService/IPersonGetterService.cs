using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using System.ComponentModel;

namespace ServiceContracts.PersonService;

/// <summary>
/// Represents a service contract for retrieving persons.
/// </summary>
public interface IPersonGetterService
{
    /// <summary>
    /// Retrieves all persons.
    /// </summary>
    /// <returns>A Task that results in a list of PersonResponse dto containing the details of all persons.</returns>
    Task<List<PersonResponse>> GetAllPersons();

    /// <summary>
    /// Retrieves a person by their unique identifier.
    /// </summary>
    /// <param name="personId">The unique identifier of the person.</param>
    /// <returns>A Task that results in a PersonResponse dto containing the details of the person, or null if not found.</returns>
    Task<PersonResponse?> GetPersonById(Guid? personId);

    /// <summary>
    /// Retrieves a list of persons based on the provided search string and search option.
    /// </summary>
    /// <param name="searchString">The search string to filter persons.</param>
    /// <param name="option">The search option to determine the filtering criteria.</param>
    /// <returns>A Task that results in a list of PersonResponse dto containing the details of the filtered persons.</returns>
    Task<List<PersonResponse>> GetFilteredPersons(string? searchString, SearchOptions option);

    /// <summary>
    /// Asynchronously generates a CSV file stream for person records.
    /// </summary>
    /// <param name="persons">Person records to convert to CSV output, or <see langword="null"/> will result in all persons being included.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="MemoryStream"/> with
    /// the generated CSV content.</returns>
    Task<MemoryStream> GetPersonsCsvFile(IEnumerable<PersonResponse>? persons = null);

    /// <summary>
    /// Asynchronously creates an Excel file containing person records.
    /// </summary>
    /// <param name="persons">The person records to include. If <see langword="null"/>, uses the default person data source.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="MemoryStream"/> with
    /// the generated Excel file.</returns>
    Task<MemoryStream> GetPersonsExcelFile(IEnumerable<PersonResponse>? persons = null);
}
