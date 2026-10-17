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

    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest, nameof(personAddRequest));

        ModelValidationHelper.Validate(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.PersonId = Guid.NewGuid();

        _persons.Add(person);

        return ConvertToPersonResponse(person);
    }

    public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        ArgumentNullException.ThrowIfNull(
            personUpdateRequest, nameof(personUpdateRequest));

        ModelValidationHelper.Validate(personUpdateRequest);

        Person? personToUpdate = 
            _persons.FirstOrDefault(p => p.PersonId == personUpdateRequest.PersonId) 
            ?? throw new ArgumentException(
                $"Person with PersonId {personUpdateRequest.PersonId} not found.",
                nameof(personUpdateRequest.PersonId));

        // Update all values except person id 
        personToUpdate.PersonName = personUpdateRequest.PersonName;
        personToUpdate.Email = personUpdateRequest.Email;
        personToUpdate.DateOfBirth = personUpdateRequest.DateOfBirth;
        personToUpdate.Gender = personUpdateRequest.Gender?.ToString();
        personToUpdate.CountryId = personUpdateRequest.CountryId;
        personToUpdate.Address = personUpdateRequest.Address;
        personToUpdate.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
        
        return ConvertToPersonResponse(personToUpdate);
    }

    public bool DeletePerson(Guid? personId)
    {
        throw new NotImplementedException();
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
        if (personsToSort == null)
        {
            throw new ArgumentNullException(nameof(personsToSort), "The collection of persons to sort cannot be null.");
        }
        if (!Enum.IsDefined(typeof(ListSortDirection), order))
        {
            throw new ArgumentOutOfRangeException(nameof(order), order, $"{nameof(order)} provided is an invalid enum for {nameof(ListSortDirection)}.");
        }

        // Non-string properties sorting (DateOfBirth and Age)
        if (option == FilterOptions.ByDateOfBirth || option == FilterOptions.ByAge)
        {
            return (option, order) switch
            {
                (FilterOptions.ByDateOfBirth, ListSortDirection.Ascending) =>
                    personsToSort.OrderBy(p => p.DateOfBirth).ToList(),

                (FilterOptions.ByDateOfBirth, ListSortDirection.Descending) =>
                    personsToSort.OrderByDescending(p => p.DateOfBirth).ToList(),

                (FilterOptions.ByAge, ListSortDirection.Ascending) =>
                    personsToSort.OrderBy(p => p.Age).ToList(),

                (FilterOptions.ByAge, ListSortDirection.Descending) =>
                    personsToSort.OrderByDescending(p => p.Age).ToList(),

                _ => throw new ArgumentOutOfRangeException(nameof(order), order, $"{nameof(order)} provided is an invalid enum for {nameof(ListSortDirection)}.")
            };
        }

        // String/Casted to string sorting for other properties
        Func<PersonResponse, string?> predicate = option switch
        {
            FilterOptions.ByName => p => p.PersonName,
            FilterOptions.ByEmail => p => p.Email,
            FilterOptions.ByGender => p => p.Gender,
            FilterOptions.ByCountry => p => p.CountryName,
            FilterOptions.ByAddress => p => p.Address,
            FilterOptions.ByReceiveNewsLetters => p => p.ReceiveNewsLetters.ToString(),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, $"{nameof(option)} provided is an invalid enum for {nameof(FilterOptions)}.")
        };
        if (order == ListSortDirection.Ascending)
        {
            return personsToSort.OrderBy(predicate, StringComparer.OrdinalIgnoreCase).ToList();
        }
        return personsToSort.OrderByDescending(predicate, StringComparer.OrdinalIgnoreCase).ToList();
    }

    // Helper method to convert a Person entity to a PersonResponse DTO, including the country name.
    private PersonResponse ConvertToPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.CountryName = _countryService
            .GetCountryById(person.CountryId)?.CountryName;
        return personResponse;
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
