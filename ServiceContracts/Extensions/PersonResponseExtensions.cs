using Entities;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;

namespace ServiceContracts.Extensions;

public static class PersonResponseExtensions
{
    /// <summary>
    /// Converts a PersonResponse DTO to a PersonUpdateRequest DTO.
    /// </summary>
    /// <param name="person">The PersonResponse DTO to convert.</param>
    /// <returns>A PersonUpdateRequest DTO.</returns>
    public static PersonUpdateRequest ToPersonUpdateRequest(this PersonResponse person)
    {
        GenderOptions? gender = Enum.TryParse<GenderOptions>(
                person.Gender, true, out var genderParsed)
                ? genderParsed : null;

        return new PersonUpdateRequest
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Gender = gender,
            CountryId = person.CountryId,
            Address = person.Address,
            ReceiveNewsLetters = person.ReceiveNewsLetters
        };
    }

}
