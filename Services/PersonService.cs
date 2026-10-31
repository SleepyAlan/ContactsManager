using CsvHelper;
using Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RepositoryContracts;
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
    private readonly IPersonsRepository _personsRepository;

    public PersonService(
        IPersonsRepository personsRepository)
    {
        _personsRepository = personsRepository;
    }

    public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest, nameof(personAddRequest));

        ModelValidationHelper.Validate(personAddRequest);

        Person person = personAddRequest.ToPerson();
        person.PersonId = Guid.NewGuid();

        var personAdded = await _personsRepository.AddPerson(person);

        return personAdded.ToPersonResponse();
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
    {
        ArgumentNullException.ThrowIfNull(
            personUpdateRequest, nameof(personUpdateRequest));

        ModelValidationHelper.Validate(personUpdateRequest);

        var personExists = await _personsRepository
            .PersonExists(personUpdateRequest.PersonId);

        if (!personExists)
        {
            throw new ArgumentException(
                $"Person with PersonId {personUpdateRequest.PersonId} does not exist.",
                nameof(personUpdateRequest.PersonId));
        }
        var personToUpdate = personUpdateRequest.ToPerson();
        var person = await _personsRepository.UpdatePerson(personToUpdate);

        if (person is null)
        {   // Fallback in case the repository returns null, which shouldn't happen if the person exists.
            throw new Exception(
                $"Person with PersonId {personUpdateRequest.PersonId} not found. Failed to update.");
        }

        return person.ToPersonResponse();
    }

    public async Task<bool> DeletePerson(Guid? personId)
    {
        ArgumentNullException.ThrowIfNull(personId, nameof(personId));

        return await _personsRepository.DeletePerson(personId.Value);
    }

    public async Task<List<PersonResponse>> GetAllPersons()
    {
        var allPersons = await _personsRepository.GetAllPersons();
        return allPersons
            .Select(person => person.ToPersonResponse())
            .ToList();
    }

    public async Task<PersonResponse?> GetPersonById(Guid? personId)
    {
        if (personId == null) return null;

        var person = await _personsRepository.GetPerson(personId.Value);

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

    public async Task<MemoryStream> GetPersonsCsvFile(IEnumerable<PersonResponse>? persons = null)
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

        persons ??= await GetAllPersons();

        await csvWriter.WriteRecordsAsync(persons);

        // Flush the CSV writer to push records to the streamWriter
        await csvWriter.FlushAsync();
        // Flush the streamWriter to push everything into the memoryStream
        await streamWriter.FlushAsync();
        // Reset the position of the memory stream to the beginning for the consumer to read from the start
        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task<MemoryStream> GetPersonsExcelFile(
        IEnumerable<PersonResponse>? persons = null)
    {
        var memoryStream = new MemoryStream();
        using var excelPackage = new ExcelPackage(memoryStream);
        var worksheet = excelPackage.Workbook.Worksheets.Add("ContactsSheet");
        persons ??= await GetAllPersons();

        // Column headers are generated from the properties of PersonResponse
        worksheet.Cells["A1"].LoadFromCollection(persons, true);

        worksheet.Column(4).Style.Numberformat.Format = "MMMM dd, yyyy";

        ExcelStylingHelper.ApplyPersonsStyling(worksheet);

        //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        await excelPackage.SaveAsync();

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
