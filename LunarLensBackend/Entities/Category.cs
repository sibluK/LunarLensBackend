using LunarLensBackend.Migrations;

namespace LunarLensBackend.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public Category(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public Category()
    {
        
    }
    
    public ICollection<Article> Articles { get; set; } = new List<Article>();
    public ICollection<Event> Events { get; set; } = new List<Event>();
    public ICollection<News> News { get; set; } = new List<News>();
    
}