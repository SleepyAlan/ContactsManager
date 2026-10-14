using ServiceContracts;
using ServiceContracts.DTOs;

namespace Services;

public class PersonService : IPersonService
{
    public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
    {
        throw new NotImplementedException();
    }

    public List<PersonResponse> GetAllPersons()
    {
        throw new NotImplementedException();
    }
}
