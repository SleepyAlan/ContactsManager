using Entities;
using ServiceContracts.DTOs;

namespace ServiceContracts.Extensions;

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