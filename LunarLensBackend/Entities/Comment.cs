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
}