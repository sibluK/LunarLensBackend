using LunarLensBackend.Database;
using LunarLensBackend.Entities;

namespace LunarLensBackend.DTOs;

public class DraftEventRequest : DraftBaseRequest
{
    public DateTime? StartDate { get; set; }
    public string? Location { get; set; }
    public string? Organizer { get; set; }
}