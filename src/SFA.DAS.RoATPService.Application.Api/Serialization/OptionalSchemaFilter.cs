using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.RoATPService.Application.Api.Serialization;

public class OptionalSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>))
        {
            var innerType = type.GetGenericArguments()[0];

            // Replace schema with inner type schema
            var innerSchema = context.SchemaGenerator.GenerateSchema(innerType, context.SchemaRepository);

            schema.Type = innerSchema.Type;
            schema.Format = innerSchema.Format;
            schema.Properties = innerSchema.Properties;
            schema.Items = innerSchema.Items;
            schema.AdditionalProperties = innerSchema.AdditionalProperties;
            schema.Nullable = true; // optional fields can be null
            schema.Reference = innerSchema.Reference;
            schema.AllOf = innerSchema.AllOf;
        }
    }
}
