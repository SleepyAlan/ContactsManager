using ServiceContracts;
using ServiceContracts.DTOs;
using Entities;
using ServiceContracts.Extensions;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using ServiceContracts.Enums;

namespace Services;

/// <summary>
/// Represents a service for managing persons, implementing the IPersonService interface.
/// </summary>
public class PersonService : IPersonService
{
    // will replace with entity framework in future, for now we will use in-memory list to store persons
    private readonly List<Person> _persons;
    private readonly ICountryService _countryService;

    public PersonService(ICountryService? service = null)
    {
        _countryService = service ?? new CountryService();
        _persons = new List<Person>();
    }

    private PersonResponse ConvertToPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.CountryName = _countryService
            .GetCountryById(person.CountryId)?.CountryName;
        return personResponse;
    }

    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest, nameof(personAddRequest));

        ModelValidationHelper.Validate(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.PersonId = Guid.NewGuid();

        _persons.Add(person);

        return ConvertToPersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
        return _persons.Select(ConvertToPersonResponse).ToList();
    }

    public PersonResponse? GetPersonById(Guid? personId)
    {
        if (personId == null) return null;
        Person? person = _persons.FirstOrDefault(
            person => person.PersonId == personId);
        return person is null ? null : ConvertToPersonResponse(person);
    }

    public List<PersonResponse> GetFilteredPersons(
        string? searchString,
        SearchOptions option)
    {
        var allPersons = GetAllPersons();
        if (string.IsNullOrEmpty(searchString) || option == SearchOptions.None)
        {
            return allPersons;
        }

        double age = 0;
        if (option == SearchOptions.GreaterThanAge ||
            option == SearchOptions.LessThanAge ||
            option == SearchOptions.EqualToAge)
        {
            if (!double.TryParse(searchString, out age))
            {
                throw new ArgumentException("Search string must be a valid number for age comparison.");
            }
        }

        switch (option)
        {
            case SearchOptions.ByName:
                return allPersons
                    .Where(p => ContainsSubstring(p.PersonName, searchString))
                    .ToList();

            case SearchOptions.ByCountry:
                return allPersons
                    .Where(p => ContainsSubstring(p.CountryName, searchString))
                    .ToList();

            case SearchOptions.ByEmail:
                return allPersons
                    .Where(p => ContainsSubstring(p.Email, searchString))
                    .ToList();

            case SearchOptions.ByAddress:
                return allPersons
                    .Where(p => ContainsSubstring(p.Address, searchString))
                    .ToList();

            case SearchOptions.ByGender:
                // Checks for exact match with GenderOptions enum values (Male, Female, Other) and filters accordingly.
                if (Enum.TryParse<GenderOptions>(searchString, true, out var genderOption))
                {
                    return allPersons
                        .Where(p => p.Gender == genderOption.ToString())
                        .ToList();
                }
                // If the search string does not match any GenderOptions enum value, perform a substring search.
                return allPersons
                    .Where(p => ContainsSubstring(p.Gender, searchString))
                    .ToList();

            case SearchOptions.GreaterThanAge:
                return allPersons
                    .Where(p => p.Age.HasValue && p.Age.Value > age)
                    .ToList();

            case SearchOptions.LessThanAge:
                return allPersons
                    .Where(p => p.Age.HasValue && p.Age.Value < age)
                    .ToList();

            case SearchOptions.EqualToAge:
                return allPersons
                    .Where(p => p.Age.HasValue && p.Age.Value == age)
                    .ToList();

            case SearchOptions.ByDateOfBirth:
                return allPersons
                    .Where(p => ContainsSubstring(p.DateOfBirth?.ToString("yyyy-MM-dd"), searchString))
                    .ToList();

            case SearchOptions.ByReceiveNewsLetters:
                bool receiveNewsLetters = searchString.Equals("true", StringComparison.OrdinalIgnoreCase);
                return allPersons
                    .Where(p => p.ReceiveNewsLetters == receiveNewsLetters)
                    .ToList();

            default: // This case should not occur due to the earlier check.
                return new List<PersonResponse>();
        }
    }

    private bool ContainsSubstring(string? toCheck, string substring)
    {
        return toCheck is not null &&
            toCheck.Contains(substring, StringComparison.OrdinalIgnoreCase);
    }
}
