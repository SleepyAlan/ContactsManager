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

    public PersonService(ICountryService? service = null, bool initialize = true)
    {
        _countryService = service ?? new CountryService(false);
        _persons = new List<Person>();
        if (initialize)
        {
            // Initialize with sample data
            _persons.AddRange(new List<Person>
            {
                new Person
                {
                    PersonId = Guid.Parse("F465D869-B488-4114-BDFF-45C696991829"),
                    PersonName = "John Doe",
                    Email = "john.doe@example.com",
                    Address = "123 Main St",
                    CountryId = Guid.Parse("1CDE0679-9CCE-47FB-B3D2-16CC0CA92C66"),
                    Gender = GenderOptions.Male.ToString(),
                    DateOfBirth = new DateTime(1990, 1, 1),
                    ReceiveNewsLetters = true
                },
                new Person
                {
                    PersonId = Guid.Parse("6499093C-C29A-4397-9DCB-75EA98E7E92A"),
                    PersonName = "Jane Smith",
                    Email = "jane.smith@example.com",
                    Address = "456 Elm St",
                    CountryId = Guid.Parse("9480C358-A557-427E-B430-0419E9522E0F"),
                    Gender = GenderOptions.Female.ToString(),
                    DateOfBirth = new DateTime(1992, 2, 2),
                    ReceiveNewsLetters = false
                },
                new Person
                {
                    PersonId = Guid.Parse("57B98B47-4F1C-4F56-9D12-5733A329480F"),
                    PersonName = "Alice Johnson",
                    Email = "alice.johnson@example.com",
                    Address = "789 Oak St",
                    CountryId = Guid.Parse("9480C358-A557-427E-B430-0419E9522E0F"),
                    Gender = GenderOptions.Female.ToString(),
                    DateOfBirth = new DateTime(1994, 3, 3),
                    ReceiveNewsLetters = true
                },
                new Person
                {
                    PersonId = Guid.Parse("D500ED4D-864F-466C-AAD9-6E9A902A7C4A"),
                    PersonName = "Bob Brown",
                    Email = "bob.brown@example.com",
                    Address = "2323 Oak St",
                    CountryId = Guid.Parse("9480C358-A557-427E-B430-0419E9522E0F"),
                    Gender = GenderOptions.Male.ToString(),
                    DateOfBirth = new DateTime(2003, 3, 3),
                    ReceiveNewsLetters = true
                },
                new Person
                {
                    PersonId = Guid.Parse("0A670657-516A-4F98-87A8-7548FE56B2D1"),
                    PersonName = "Alan Sam",
                    Email = "alan.sam@example.com",
                    Address = "2323 Oak St",
                    CountryId = Guid.Parse("DBF05F1A-FDE3-47D2-AB7E-0DDFF2B662AC"),
                    Gender = GenderOptions.Male.ToString(),
                    DateOfBirth = new DateTime(1992, 3, 3),
                    ReceiveNewsLetters = false
                },
                new Person
                {
                    PersonId = Guid.Parse("BC8C2292-8504-4322-8D10-7A8B8A3D5B83"),
                    PersonName = "Brice Johnson",
                    Email = "brice.johnson@example.com",
                    Address = "new street",
                    CountryId = Guid.Parse("9480C358-A557-427E-B430-0419E9522E0F"),
                    Gender = GenderOptions.Male.ToString(),
                    DateOfBirth = new DateTime(1994, 3, 3),
                    ReceiveNewsLetters = true
                },
                new Person
                {
                    PersonId = Guid.Parse("1DAF5F6D-9B83-4FE1-89BE-8F7100025000"),
                    PersonName = "Tyler Sire",
                    Email = "tyler.sire@example.com",
                    Address = "4339 New Haven St",
                    CountryId = Guid.Parse("9480C358-A557-427E-B430-0419E9522E0F"),
                    Gender = GenderOptions.Male.ToString(),
                    DateOfBirth = new DateTime(1994, 12, 4),
                    ReceiveNewsLetters = false
                },
                new Person
                {
                    PersonId = Guid.Parse("331172CB-E213-48F1-A300-B096A1AF550C"),
                    PersonName = "Nick Holt",
                    Email = "nick.holt@example.com",
                    Address = "789 Oak St",
                    CountryId = Guid.Parse("364BF47F-45AF-4283-B44E-9890C6ED46BB"),
                    Gender = GenderOptions.Male.ToString(),
                    DateOfBirth = new DateTime(2008, 2, 4),
                    ReceiveNewsLetters = false
                },
                new Person
                {
                    PersonId = Guid.Parse("6000C71B-F224-4BCE-8C98-2E890EF81EC1"),
                    PersonName = "Alyssa Holmes",
                    Email = "alyssa.holmes@example.com",
                    Address = "100 Maple St",
                    CountryId = Guid.Parse("B7F76A6B-5B0D-4302-9D82-8BFFF29D2F98"),
                    Gender = GenderOptions.Female.ToString(),
                    DateOfBirth = null,
                    ReceiveNewsLetters = false
                },
                new Person
                {
                    PersonId = Guid.Parse("36167B0D-8A9E-4D70-8217-A2C5498C2D16"),
                    PersonName = "Bryce Hall",
                    Email = "bryce.hall@example.com",
                    Address = "123 Maple St",
                    CountryId = Guid.Parse("B7F76A6B-5B0D-4302-9D82-8BFFF29D2F98"),
                    Gender = GenderOptions.Male.ToString(),
                    DateOfBirth = new DateTime(2006, 8, 4),
                    ReceiveNewsLetters = false
                },
            });
        }
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest, nameof(personAddRequest));

        ModelValidationHelper.Validate(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.PersonId = Guid.NewGuid();

        _persons.Add(person);

        return await ConvertToPersonResponse(person);
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
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
        
        return await ConvertToPersonResponse(personToUpdate);
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        ArgumentNullException.ThrowIfNull(personId, nameof(personId));

        Person? person = _persons.FirstOrDefault(
            person => person.PersonId == personId);
        if (person == null) return false;

        int numberOfRemoved = _persons.RemoveAll(p => p.PersonId == personId);
        return numberOfRemoved > 0;
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        var tasks = _persons.Select(ConvertToPersonResponse);
        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    public async Task<PersonResponse?> GetPersonById(Guid? personId)
    {
        if (personId == null) return null;
        Person? person = _persons.FirstOrDefault(
            person => person.PersonId == personId);
        return person is null ? null : await ConvertToPersonResponse(person);
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string? searchString, SearchOptions option)
    {
        var allPersons = await GetAllPersons();

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
    private async Task<PersonResponse> ConvertToPersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.CountryName = (await _countryService
            .GetCountryById(person.CountryId))?.CountryName;
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
            SearchOptions.ByDateOfBirth => p => (DateTimeSearch.Matcher(p.DateOfBirth, searchString)
                || ContainsSubstring(p.DateOfBirth?.ToString("yyyy-MM-dd"), searchString)),
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
