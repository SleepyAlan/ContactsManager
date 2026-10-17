using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs;

/// <summary>
/// Represents the dto for updating a person's details/data.
/// </summary>
public class PersonUpdateRequest
{
    [Required(ErrorMessage = "PersonId cannot be null or empty.")]
    public Guid PersonId { get; set; }
    [Required(ErrorMessage = "PersonName cannot be null, empty, or whitespace.")]
    [MaxLength(100, ErrorMessage = "PersonName cannot exceed 100 characters.")]
    public string? PersonName { get; set; }
    [Required(ErrorMessage = "Email cannot be null, empty, or whitespace.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public GenderOptions? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }
}
