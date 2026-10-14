using ServiceContracts;
using ServiceContracts.DTOs;
using Entities;
using ServiceContracts.Extensions;

namespace Services;

public class PersonService : IPersonService
{
    private readonly List<Person> _persons = new();

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
        return person.ToPersonResponse();
    }

    public List<PersonResponse> GetAllPersons()
    {
        return _persons
            .Select(person => person.ToPersonResponse())
            .ToList();
    }
}
