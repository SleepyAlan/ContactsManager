using Entities;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;

namespace ServiceContracts.Extensions;

public static class PersonResponseExtensions
{
    /// <summary>
    /// Converts a Person entity to a PersonUpdateRequest DTO.
    /// </summary>
    /// <param name="person">The Person entity to convert.</param>
    /// <returns>A PersonUpdateRequest DTO.</returns>
    public static PersonUpdateRequest ToPersonUpdateRequest(this Person person)
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
