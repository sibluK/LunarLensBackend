namespace LunarLensBackend.Entities.Shared;

public class ContentSection
{
    public int Id { get; set; } 
    public string Text { get; set; } 
    public byte[]? Image { get; set; } 
    
    public int ContentBaseId { get; set; } // Foreign key to ContentBase
    public ContentBase ContentBase { get; set; } // Navigation property
}