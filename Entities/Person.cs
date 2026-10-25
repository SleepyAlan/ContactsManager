using System.ComponentModel.DataAnnotations;

namespace Entities;

public class Person
{
    [Key]
    public Guid PersonId { get; set; }

    [StringLength(150)]
    public string? PersonName { get; set; }

    [StringLength(254)]
    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    public Guid? CountryId { get; set; }

    [StringLength(400)]
    public string? Address { get; set; }

    public bool ReceiveNewsLetters { get; set; }

    public Country? Country { get; set; }
}
