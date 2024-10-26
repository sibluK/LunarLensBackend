using System.Text.Json.Serialization;
using LunarLensBackend.Entities.Enums;

namespace LunarLensBackend.DTOs;

public class ChangeContentStatusRequest
{
    public required int ContentId { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ContentType Type { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ContentStatus Status { get; set; }
}