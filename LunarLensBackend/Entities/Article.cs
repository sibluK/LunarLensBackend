using LunarLensBackend.Entities.Enums;
using LunarLensBackend.Entities.Shared;

namespace LunarLensBackend.Entities;

public class Article : ContentBase
{
    public Article()
    {
        Type = ContentType.Article;
    }
    
}