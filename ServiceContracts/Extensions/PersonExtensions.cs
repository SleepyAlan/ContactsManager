using ServiceContracts.DTOs;
using Entities;

namespace ServiceContracts.Extensions;

public static class PersonExtensions
{
    /// <summary>
    /// Converts a Person entity to a PersonResponse DTO.
    /// </summary>
    /// <param name="person">The Person entity to convert.</param>
    /// <returns>A PersonResponse DTO.</returns>
    public static PersonResponse ToPersonResponse(this Person person)
    {
        double? age = person.DateOfBirth.HasValue ? (DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25 : null;
        //string? countryName = person.Country?.CountryName;
        return new PersonResponse
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Age = age,
            Gender = person.Gender,
            CountryId = person.CountryId,
            //CountryName = person.Country?.CountryName,
            Address = person.Address,
            ReceiveNewsLetters = person.ReceiveNewsLetters
        };
    }
}