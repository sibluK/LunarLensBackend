using LunarLensBackend.Entities.Enums;
using LunarLensBackend.Entities.Shared;

namespace LunarLensBackend.Entities;

public class News : ContentBase
{
    public News()
    {
        Type = ContentType.News;
    }
    
    public string Source { get; set; }
}