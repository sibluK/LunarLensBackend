using LunarLensBackend.Database;
using LunarLensBackend.Entities;

namespace LunarLensBackend.DTOs;

public class DraftBaseRequest
{
    public string Title { get; set; }
    public string? Summary { get; set; }
    public byte[]? Image { get; set; }
    public ICollection<ContentSectionDTO>? ContentSections { get; set; } = new List<ContentSectionDTO>();
    public ICollection<WriterDTO>? Writers { get; set; } = new List<WriterDTO>();
    public ICollection<CategoryDTO>? Categories { get; set; } = new List<CategoryDTO>();
}