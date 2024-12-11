namespace LunarLensBackend.Entities;

public class ContentSection
{
    public int Id { get; set; } 
    public string Text { get; set; } 
    public byte[]? Image { get; set; }

    public ContentSection(int id, string text, byte[]? image)
    {
        Id = id;
        Text = text;
        Image = image;
    }

    public ContentSection()
    {
        
    }
    
}