using ServiceContracts.enums;

namespace ServiceContracts.DTOs;

/// <summary>
/// Represents a DTO for inserting a new person.
/// </summary>
public class PersonAddRequest
{
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
}
