using LunarLensBackend.Entities.Enums;
using LunarLensBackend.Entities.Shared;

namespace LunarLensBackend.Entities;

public class Event : ContentBase
{
    public Event()
    {
        Type = ContentType.Event;
    }
    
    public DateTime StartDate { get; set; }
    public string Location { get; set; }
    public string Organizer { get; set; }
}