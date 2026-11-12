using ServiceContracts.DTOs;
using ServiceContracts.Enums;
using ServiceContracts.PersonService;
using System.ComponentModel;

namespace ServiceContracts;

/// <summary>
/// Represents a service contract for CRUD operations on managing persons.
/// </summary>
public interface IPersonService :
    IPersonAdderService,
    IPersonUpdaterService,
    IPersonDeleterService,
    IPersonGetterService,
    IPersonSorterService
{
}
