namespace LunarLensBackend.Entities;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public bool IsEdited { get; set; } = false;
    public string UserId { get; set; }
    
    // Foreign keys for relationships with Event, News, and Article
    public int? EventId { get; set; }
    public int? NewsId { get; set; }
    public int? ArticleId { get; set; }

    // Navigation properties for EF relationships
    public Event Event { get; set; }
    public News News { get; set; }
    public Article Article { get; set; }


    public Comment(string text, string userId, int? articleId, int? eventId, int? newsId)
    {
        Text = text;
        UserId = userId;
        EventId = eventId;
        NewsId = newsId;
        ArticleId = articleId;
    }

    public Comment()
    {
        
    }
    
}