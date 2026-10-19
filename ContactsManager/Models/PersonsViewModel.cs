using ServiceContracts.DTOs;

namespace ContactsManager.Models;

public class PersonsViewModel
{
    public string? SearchString { get; set; }
    public string? SelectedSearchField { get; set; }
    public string? SortColumn { get; set; }
    public string? SortDirection { get; set; }
    public IEnumerable<PersonResponse?> Persons { get; set; }
    public Dictionary<string, string> SearchFields { get; set; }
    public List<ValueTuple<string, string>> SortFields { get; set; }

    public PersonsViewModel(
        IEnumerable<PersonResponse?> persons,
        Dictionary<string, string> searchFields,
        List<ValueTuple<string, string>> sortFields,
        string? selectedSearchField = null,
        string? searchString = null,
        string? sortColumn = null,
        string? sortDirection = null)

    {
        Persons = persons;
        SearchFields = searchFields;
        SortFields = sortFields;

        SelectedSearchField = selectedSearchField;
        SearchString = searchString;
        SortColumn = sortColumn;
        SortDirection = sortDirection;
    }
}
