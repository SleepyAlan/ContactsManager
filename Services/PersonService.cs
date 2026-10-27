using CsvHelper;
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ServiceContracts.Extensions;
using Services.Helpers;
using System.ComponentModel;
using System.Globalization;

namespace Services;

public class PersonService : IPersonService
{
    private readonly ContactsManagerDbContext _db;

    public PersonService(
        ContactsManagerDbContext dbContext)
    {
        _db = dbContext;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest, nameof(personAddRequest));

        ModelValidationHelper.Validate(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.PersonId = Guid.NewGuid();

        var entityEntry = _db.Persons.Add(person);
        await _db.SaveChangesAsync();

        if (person.CountryId.HasValue)
        {
            // Loading navigational property
            await entityEntry
                .Reference(p => p.Country)
                .LoadAsync();
        }
        return person.ToPersonResponse();
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        ArgumentNullException.ThrowIfNull(
            personUpdateRequest, nameof(personUpdateRequest));

        ModelValidationHelper.Validate(personUpdateRequest);

        Person? personToUpdate = await _db.Persons
            .Include(p => p.Country)
            .FirstOrDefaultAsync(p => p.PersonId == personUpdateRequest.PersonId) 
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

        await _db.SaveChangesAsync();

        return personToUpdate.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        ArgumentNullException.ThrowIfNull(personId, nameof(personId));

        Person? person = await _db.Persons
            .FirstOrDefaultAsync(person => person.PersonId == personId);

        if (person == null) return false;

        _db.Persons.Remove(person);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        return await _db.Persons
            .Include(p => p.Country)
            .Select(p => p.ToPersonResponse())
            .ToListAsync();
    }

    public async Task<PersonResponse?> GetPersonById(Guid? personId)
    {
        if (personId == null) return null;

        Person? person = await _db.Persons
            .Include(p => p.Country)
            .FirstOrDefaultAsync(
                person => person.PersonId == personId);

        return person is null ? null : person.ToPersonResponse();
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

    public async Task<MemoryStream> GetPersonsCsvFile()
    {
        var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(
            memoryStream,
            leaveOpen: true);
        using var csvWriter = new CsvWriter(
            streamWriter,
            CultureInfo.InvariantCulture,
            leaveOpen: true);

        csvWriter.WriteHeader<PersonResponse>();
        await csvWriter.NextRecordAsync();

        var persons = await GetAllPersons();

        await csvWriter.WriteRecordsAsync(persons);

        // Flush the CSV writer to push records to the streamWriter
        await csvWriter.FlushAsync();
        // Flush the streamWriter to push everything into the memoryStream
        await streamWriter.FlushAsync();
        // Reset the position of the memory stream to the beginning for the consumer to read from the start
        memoryStream.Position = 0;

        return memoryStream;
    }

    #region Private Helper Methods for Building Predicates
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
    #endregion
}
