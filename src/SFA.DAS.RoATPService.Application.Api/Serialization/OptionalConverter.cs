#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.RoATPService.Application.Api.Serialization;

public class OptionalConverter<T> : JsonConverter<Optional<T>>
{
    public override Optional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Handle explicit null
        if (reader.TokenType == JsonTokenType.Null)
        {
            return Optional<T>.FromValue(default!);
        }

        // Handle enums (string or number)
        Type underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (underlyingType.IsEnum)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (string.IsNullOrWhiteSpace(str))
                {
                    // present, but cleared => HasValue = true, Value = null (nullable enum)
                    return Optional<T>.FromValue((T?)(object?)null);
                }

                // parse enum name (case-insensitive)
                var parsed = Enum.Parse(underlyingType, str!, ignoreCase: true);
                return Optional<T>.FromValue((T)parsed);
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out var intValue))
                {
                    var enumObj = Enum.ToObject(underlyingType, intValue);
                    return Optional<T>.FromValue((T)enumObj);
                }

                throw new JsonException($"Invalid number for enum {underlyingType.Name}");
            }

            throw new JsonException($"Unexpected token {reader.TokenType} for enum {underlyingType.Name}");
        }

        // Fallback for all other types
        T? deserialized = JsonSerializer.Deserialize<T>(ref reader, options);
        return Optional<T>.FromValue(deserialized);
    }

    public override void Write(Utf8JsonWriter writer, Optional<T> value, JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            // Normally we omit missing fields — writing null is safe default
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value.Value, options);
    }
}
