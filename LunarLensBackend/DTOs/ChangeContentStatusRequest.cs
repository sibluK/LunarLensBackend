using System.Text.Json.Serialization;
using LunarLensBackend.Entities.Enums;
using LunarLensBackend.Utility;

namespace LunarLensBackend.DTOs;

public class ChangeContentStatusRequest
{
    public required int ContentId { get; set; }
    
    [JsonConverter(typeof(SafeEnumConverter<ContentType>))]
    [EnumValidation(typeof(ContentType))]
    public required ContentType Type { get; set; }

    [JsonConverter(typeof(SafeEnumConverter<ContentStatus>))]
    [EnumValidation(typeof(ContentStatus))]
    public required ContentStatus Status { get; set; }
}