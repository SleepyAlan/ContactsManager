namespace ServiceContracts.DTOs;

/// <summary>
/// Represents response DTO for person and is the return type of most methods of the service.
/// </summary>
public class PersonResponse
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
        return obj is PersonResponse response &&
               PersonId == response.PersonId &&
               PersonName == response.PersonName &&
               Email == response.Email &&
               DateOfBirth == response.DateOfBirth &&
               Age == response.Age &&
               Gender == response.Gender &&
               CountryId == response.CountryId &&
               CountryName == response.CountryName &&
               Address == response.Address &&
               ReceiveNewsLetters == response.ReceiveNewsLetters;
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
