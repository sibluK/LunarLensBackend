using System.Net.Mime;
using LunarLensBackend.Entities.Shared;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Database;

public class ApplicationUser : IdentityUser
{
    public string? MicrosoftId { get; set; }
    public byte[]? Image { get; set; } 
    
    // Navigation properties
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<ContentBase> WrittenContents { get; set; } = new List<ContentBase>();
    
    public string? GetImageAsBase64()
    {
        return Image != null ? $"data:image/png;base64,{Convert.ToBase64String(Image)}" : null;
    }
}

