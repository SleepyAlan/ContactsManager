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
        int age = -1;
        bool isAgeCalculated = false;
        if (person.DateOfBirth is DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }
            isAgeCalculated = true;
        }
        
        return new PersonResponse
        {
            PersonId = person.PersonId,
            PersonName = person.PersonName,
            Email = person.Email,
            DateOfBirth = person.DateOfBirth,
            Age = isAgeCalculated ? age : null,
            Gender = person.Gender,
            CountryId = person.CountryId,
            CountryName = person.Country?.CountryName,
            Address = person.Address,
            ReceiveNewsLetters = person.ReceiveNewsLetters
        };
    }
}