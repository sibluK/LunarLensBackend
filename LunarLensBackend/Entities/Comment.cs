using System.Net.Mime;
using LunarLensBackend.Database;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
}