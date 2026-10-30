using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs;

public class CountryUpdateRequest
{
    [Required(ErrorMessage = "CountryId cannot be null or empty.")]
    public Guid CountryId { get; set; }

    [Required(ErrorMessage = "CountryName cannot be null, empty, or whitespace.")]
    public string? CountryName { get; set; }
}
