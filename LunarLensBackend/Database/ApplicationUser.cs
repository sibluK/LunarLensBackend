using LunarLensBackend.Entities;
using Microsoft.AspNetCore.Identity;

namespace LunarLensBackend.Database;

public class ApplicationUser : IdentityUser
{
    public string? MicrosoftId { get; set; }
    public byte[]? Image { get; set; } 
    
    public ICollection<Article> Articles { get; set; } = new List<Article>();
    public ICollection<News> News { get; set; } = new List<News>();
    public ICollection<Event> Events { get; set; } = new List<Event>();
    
    public string? GetImageAsBase64()
    {
        return Image != null ? $"data:image/png;base64,{Convert.ToBase64String(Image)}" : null;
    }

    public void addArticle(Article article)
    {
        Articles.Add(article);
    }

    public void addNews(News news)
    {
        News.Add(news);
    }

    public void removeArticle(Article article)
    {
        Articles.Remove(article);
    }

    public void removeNews(News news)
    {
        News.Remove(news);
    }
}

