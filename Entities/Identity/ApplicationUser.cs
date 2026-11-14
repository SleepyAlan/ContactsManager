using Microsoft.AspNetCore.Identity;

namespace Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
}
