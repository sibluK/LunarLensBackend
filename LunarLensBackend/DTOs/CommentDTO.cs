namespace LunarLensBackend.DTOs;

public class CommentDTO
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public DateTime Date { get; set; } 
    public bool IsEdited { get; set; }
    public string UserId { get; set; }
    
    public int? ContentId { get; set; }

}