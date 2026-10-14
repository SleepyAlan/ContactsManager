using ServiceContracts;
using ServiceContracts.DTOs;
using Entities;
using ServiceContracts.Extensions;

namespace Services;

/// <summary>
/// Represents a service for managing persons, implementing the IPersonService interface.
/// </summary>
public class PersonService : IPersonService
{
    // will replace with entity framework in future, for now we will use in-memory list to store persons
    private readonly List<Person> _persons;
    private readonly ICountryService _countryService;

    public PersonService()
    {
        _countryService = new CountryService();
        _persons = new List<Person>();
    }

    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        ArgumentNullException.ThrowIfNull(personAddRequest, nameof(personAddRequest));

        if (string.IsNullOrWhiteSpace(personAddRequest.PersonName))
        {
            throw new ArgumentException(
                "PersonName cannot be null, empty, or whitespace.",
                nameof(personAddRequest.PersonName));
        }

        Person person = personAddRequest.ToPerson();
        person.PersonId = Guid.NewGuid();

        _persons.Add(person);

        return GeneratePersonResponse(person);
    }

    public List<PersonResponse> GetAllPersons()
    {
        return _persons.Select(GeneratePersonResponse).ToList();
    }

    private PersonResponse GeneratePersonResponse(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.CountryName = _countryService
            .GetCountryByCountryId(person.CountryId)?.CountryName;
        return personResponse;
    }
}
