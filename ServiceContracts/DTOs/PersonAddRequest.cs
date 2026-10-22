using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs;

/// <summary>
/// Represents a DTO for inserting a new person.
/// </summary>
public class PersonAddRequest
{

    // PersonName cannot be null, empty, or whitespace. It must not exceed 100 characters in length.
    [Required(ErrorMessage = "Full Name cannot be empty or whitespace.")]
    [MaxLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    public string? PersonName { get; set; }
    [Required(ErrorMessage = "Email cannot be empty or whitespace.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
}
