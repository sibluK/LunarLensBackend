using System.Net.Mime;
using LunarLensBackend.Database;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Entities.Shared;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public DateTime Date { get; set; }
    
    // Navigation properties
    public int ContentBaseId { get; set; } // Foreign key to ContentBase
    public ContentBase Content { get; set; }
    public string UserId { get; set; }  // FK to ApplicationUser
    public ApplicationUser User { get; set; }
}