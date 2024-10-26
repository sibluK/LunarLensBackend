using System.Net.Mime;
using LunarLensBackend.Entities.Shared;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Database;

public class ApplicationUser : IdentityUser
{
    public string? MicrosoftId { get; set; }
    public byte[]? Image { get; set; } 
    
    public string? GetImageAsBase64()
    {
        return Image != null ? $"data:image/png;base64,{Convert.ToBase64String(Image)}" : null;
    }
}

