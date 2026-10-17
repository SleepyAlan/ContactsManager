using Entities;
using ServiceContracts.DTOs;

namespace ServiceContracts.Extensions;

public static class PersonUpdateRequestExtensions
{
    /// <summary>
    /// Converts a PersonUpdateRequest object to a Person entity.
    /// </summary>
    /// <param name="personUpdateRequest">The PersonUpdateRequest object to convert.</param>
    /// <returns>A Person entity.</returns>
    public static Person ToPerson(this PersonUpdateRequest personUpdateRequest)
    {
        return new Person
        {
            PersonId = personUpdateRequest.PersonId,
            PersonName = personUpdateRequest.PersonName,
            Email = personUpdateRequest.Email,
            DateOfBirth = personUpdateRequest.DateOfBirth,
            Gender = personUpdateRequest.Gender.ToString(),
            CountryId = personUpdateRequest.CountryId,
            Address = personUpdateRequest.Address,
            ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters
        };
    }
}