using ServiceContracts.enums;
using Entities;

namespace ServiceContracts.DTOs;

/// <summary>
/// Represents a DTO for inserting a new person.
/// </summary>
public class PersonAddRequest
{
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
}

public static class PersonAddRequestExtensions
{
    /// <summary>
    /// Converts a PersonAddRequest object to a Person entity.
    /// </summary>
    /// <param name="personAddRequest">The PersonAddRequest object to convert.</param>
    /// <returns>A Person entity.</returns>
    public static Person ToPerson(this PersonAddRequest personAddRequest)
    {
        return new Person
        {
            PersonName = personAddRequest.PersonName,
            Email = personAddRequest.Email,
            DateOfBirth = personAddRequest.DateOfBirth,
            Gender = personAddRequest.Gender.ToString(),
            CountryId = personAddRequest.CountryId,
            Address = personAddRequest.Address,
            ReceiveNewsLetters = personAddRequest.ReceiveNewsLetters
        };
    }
}