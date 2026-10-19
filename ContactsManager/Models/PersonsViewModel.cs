using ServiceContracts.DTOs;

namespace ContactsManager.Models;

public class PersonsViewModel
{
    public string? SelectedSearchField { get; set; }
    public string? SearchString { get; set; }
    public IEnumerable<PersonResponse?> Persons { get; set; }
    public Dictionary<string, string> SearchFields { get; set; }

    public PersonsViewModel(IEnumerable<PersonResponse?> persons, Dictionary<string, string> searchFields, string? selectedSearchField = null, string? searchString = null)

    {
        Persons = persons;
        SearchFields = searchFields;
        SelectedSearchField = selectedSearchField;
        SearchString = searchString;
    }
}
