namespace ServiceContracts.DTOs;

/// <summary>
/// Represents the response DTO for Country entity.
/// It is a return type of most CountriesService methods.
/// </summary>
public class CountryResponse : IEquatable<CountryResponse?>
{
    public Guid CountryId { get; set; }
    public string? CountryName { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CountryResponse);
    }

    public bool Equals(CountryResponse? other)
    {
        return other is not null &&
               CountryId.Equals(other.CountryId) &&
               CountryName == other.CountryName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CountryId, CountryName);
    }
}
