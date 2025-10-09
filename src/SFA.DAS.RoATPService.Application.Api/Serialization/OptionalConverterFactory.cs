#nullable enable

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.RoATPService.Application.Api.Serialization;

public class OptionalConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OptionalConverter<>).MakeGenericType(itemType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

