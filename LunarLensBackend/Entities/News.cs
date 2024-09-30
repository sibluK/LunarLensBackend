namespace LunarLensBackend.Entities;

public class News
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public int Views { get; set; }
    public byte[] Image { get; set; }
    public DateTime PublishedDate { get; set; }
    
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<ContentSection> ContentSections { get; set; } = new List<ContentSection>();
    public List<string> Author { get; set; } =  new List<string>();
    public List<Category> Categories { get; set; } =  new List<Category>();
}