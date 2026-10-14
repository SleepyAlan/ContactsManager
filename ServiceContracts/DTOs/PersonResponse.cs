namespace ServiceContracts.DTOs;

/// <summary>
/// Represents response DTO for person and is the return type of most methods of the service.
/// </summary>
public class PersonResponse : IEquatable<PersonResponse?>
{
    public Guid PersonId { get; set; }
    public string? PersonName { get; set; }
    public string? Email { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public double? Age { get; set; }   
    public string? Gender { get; set; }
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? Address { get; set; }
    public bool ReceiveNewsLetters { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PersonResponse);
    }

    public bool Equals(PersonResponse? other)
    {
        return other is not null &&
               PersonId == other.PersonId &&
               PersonName == other.PersonName &&
               Email == other.Email &&
               DateOfBirth == other.DateOfBirth &&
               Gender == other.Gender &&
               CountryId == other.CountryId &&
               CountryName == other.CountryName &&
               Address == other.Address &&
               ReceiveNewsLetters == other.ReceiveNewsLetters;
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(PersonId);
        hash.Add(PersonName);
        hash.Add(Email);
        hash.Add(DateOfBirth);
        hash.Add(Age);
        hash.Add(Gender);
        hash.Add(CountryId);
        hash.Add(CountryName);
        hash.Add(Address);
        hash.Add(ReceiveNewsLetters);
        return hash.ToHashCode();
    }
}
