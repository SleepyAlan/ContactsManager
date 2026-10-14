namespace ServiceContracts.DTOs;

/// <summary>
/// Represents a DTO for inserting a new country.
/// </summary>
public class CountryAddRequest
{
    public string? CountryName { get; set; }
}
