using ServiceContracts;
using ServiceContracts.DTOs;
using Entities;
using ServiceContracts.Extensions;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using ServiceContracts.Enums;
using System.ComponentModel;

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

    public List<PersonResponse> GetFilteredPersons(string? searchString, SearchOptions option)
    {
        var allPersons = GetAllPersons();

        if (string.IsNullOrEmpty(searchString))
        {
            return allPersons;
        }

        Func<PersonResponse, bool> predicate = BuildPredicate(searchString, option);

        return allPersons.Where(predicate).ToList();
    }

    public List<PersonResponse> GetSortedPersons(
        IEnumerable<PersonResponse> personsToSort,
        FilterOptions option,
        ListSortDirection order = ListSortDirection.Ascending)
    {
        Func<PersonResponse, object?> predicate = option switch
        {
            FilterOptions.ByName => p => p.PersonName,
            FilterOptions.ByEmail => p => p.Email,
            FilterOptions.ByDateOfBirth => p => p.DateOfBirth,
            FilterOptions.ByAge => p => p.Age,
            FilterOptions.ByGender => p => p.Gender,
            FilterOptions.ByCountry => p => p.CountryName,
            FilterOptions.ByAddress => p => p.Address,
            FilterOptions.ByReceiveNewsLetters => p => p.ReceiveNewsLetters,
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, $"{nameof(option)} provided is an invalid enum for {nameof(FilterOptions)}.")
        };
        if (order == ListSortDirection.Ascending)
        {
            return personsToSort.OrderBy(predicate).ToList();
        }
        return personsToSort.OrderByDescending(predicate).ToList();
    }

    // Below are private helper methods to build predicates for filtering based on different search options.

    private static Func<PersonResponse, bool> BuildPredicate(
        string searchString, SearchOptions option)
    {
        return option switch
        {
            SearchOptions.ByName => p => ContainsSubstring(p.PersonName, searchString),
            SearchOptions.ByCountry => p => ContainsSubstring(p.CountryName, searchString),
            SearchOptions.ByEmail => p => ContainsSubstring(p.Email, searchString),
            SearchOptions.ByAddress => p => ContainsSubstring(p.Address, searchString),
            SearchOptions.ByDateOfBirth => p => ContainsSubstring(
                p.DateOfBirth?.ToString("yyyy-MM-dd"), searchString),
            SearchOptions.ByGender => BuildGenderPredicate(searchString),
            SearchOptions.GreaterThanAge => BuildAgePredicate(searchString, (x, y) => x > y),
            SearchOptions.LessThanAge => BuildAgePredicate(searchString, (x, y) => x < y),
            SearchOptions.EqualToAge => BuildAgePredicate(searchString, (x, y) => x == y),
            SearchOptions.ByReceiveNewsLetters => BuildNewsLetterPredicate(searchString),
            _ => throw new ArgumentOutOfRangeException(
                    nameof(option), option, "Unsupported search option.")
        };
    }

    private static bool ContainsSubstring(string? toCheck, string substring)
    {
        return toCheck is not null &&
            toCheck.Contains(substring, StringComparison.OrdinalIgnoreCase);
    }

    private static Func<PersonResponse, bool> BuildGenderPredicate(string search)
    {
        if (Enum.TryParse<GenderOptions>(search, true, out var gender))
        {
            var genderName = gender.ToString();
            return personResponse =>
                genderName.Equals(personResponse.Gender, StringComparison.OrdinalIgnoreCase);
        }
        return personResponse => ContainsSubstring(personResponse.Gender, search);
    }

    private static Func<PersonResponse, bool> BuildAgePredicate(
        string search,
        Func<int, int, bool> compare)
    {
        if (!int.TryParse(search, out var searchAsInt))
        {
            throw new ArgumentException("Search string must be a valid number for age comparison.", nameof(search));
        }
        return personResponse =>
            personResponse.Age.HasValue &&
            compare(personResponse.Age.Value, searchAsInt);
    }

    private static Func<PersonResponse, bool> BuildNewsLetterPredicate(string search)
    {
        bool result = search.Equals("true", StringComparison.OrdinalIgnoreCase);
        return personResponse => personResponse.ReceiveNewsLetters == result;
    }
}
