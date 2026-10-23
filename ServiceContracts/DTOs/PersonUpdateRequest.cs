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
    [StringLength(150, ErrorMessage = "PersonName cannot exceed 150 characters.")]
    public string? PersonName { get; set; }

    [Required(ErrorMessage = "Email cannot be null, empty, or whitespace.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    [StringLength(254, ErrorMessage = "Email cannot exceed 254 characters.")]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public GenderOptions? Gender { get; set; }

    public Guid? CountryId { get; set; }

    [StringLength(400, ErrorMessage = "Address cannot exceed 400 characters.")]
    public string? Address { get; set; }

    public bool ReceiveNewsLetters { get; set; }
}
