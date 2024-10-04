using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Database;

public class ApplicationUser : IdentityUser
{
    public string? MicrosoftId { get; set; }
}