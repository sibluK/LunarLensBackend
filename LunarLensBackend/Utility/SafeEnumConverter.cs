using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunarLensBackend.Utility;

public class SafeEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var enumString = reader.GetString();
            if (Enum.TryParse(typeof(T), enumString, true, out var value))
            {
                return (T)value;
            }

            throw new JsonException($"Invalid value \"{enumString}\" for enum {typeof(T).Name}. Allowed values are: {string.Join(", ", Enum.GetNames(typeof(T)))}.");
        }

        throw new JsonException($"Expected string for enum {typeof(T).Name}, but got {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}