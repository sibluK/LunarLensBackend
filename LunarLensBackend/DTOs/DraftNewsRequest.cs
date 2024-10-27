using LunarLensBackend.Database;
using LunarLensBackend.Entities;

namespace LunarLensBackend.DTOs;

public class DraftNewsRequest : DraftBaseRequest
{
    public string? Source { get; set; }
}