using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs;

public class RegisterDTO
{
    [Required(ErrorMessage = "Display Name is required.")]
    public string? DisplayName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone is required.")]
    [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid Phone Number.")]
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required.")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}
