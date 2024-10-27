using System.Text.RegularExpressions;
using LunarLensBackend.Database;
using LunarLensBackend.Entities.Enums;

namespace LunarLensBackend.Entities;

public class Article
{
    public int Id { get; set; }
    private string _title;
    public string? Title
    {
        get => _title;
        set
        {
            _title = value;
            Slug = value != null ? GenerateSlug(value) : null;
        }
    }
    public string? Slug { get; private set; }
    public string? Summary { get; set; }
    public int? Views { get; set; } = 0;
    public int? Likes { get; set; } = 0;
    public int? Dislikes { get; set; } = 0;
    public byte[]? Image { get; set; }
    public DateTime? PublishedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; } = DateTime.UtcNow;
    public ContentStatus? Status { get; set; } = ContentStatus.Drafted;
    public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
    public ICollection<ContentSection>? ContentSections { get; set; } = new List<ContentSection>();
    public ICollection<ApplicationUser>? Writers { get; set; } = new List<ApplicationUser>();
    public ICollection<Category>? Categories { get; set; } = new List<Category>();

    private string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return string.Empty;
        
        title = title.ToLowerInvariant();
        
        title = title.Replace(" ", "-");

        // Remove invalid URL characters
        title = Regex.Replace(title, @"[^a-z0-9\-]", string.Empty);

        return title;
    }
    
}