namespace ServiceContracts.DTOs;

/// <summary>
/// Represents the response DTO for Country entity.
/// It is a return type of most CountriesService methods.
/// </summary>
public class CountryResponse
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is CountryResponse response &&
               CountryId.Equals(response.CountryId) &&
               CountryName == response.CountryName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CountryId, CountryName);
    }
}
