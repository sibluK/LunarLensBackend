using System.Net.Mime;
using LunarLensBackend.Database;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime Date { get; set; }
    public ApplicationUser User { get; set; }
}