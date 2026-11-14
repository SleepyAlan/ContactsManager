namespace ContactsManager.Models;

public class LoginRequestViewModel
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool RememberMe { get; set; } = false; // Default value is false
}
