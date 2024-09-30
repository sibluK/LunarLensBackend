using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public IdentityUser Author { get; set; }
}