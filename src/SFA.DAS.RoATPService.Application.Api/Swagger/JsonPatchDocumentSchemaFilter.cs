using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.RoATPService.Application.Api.Swagger;

public class JsonPatchDocumentSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.Name.StartsWith("JsonPatchDocument"))
        {
            schema.Type = "array";
            schema.Items = new OpenApiSchema
            {
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["op"] = new OpenApiSchema { Type = "string", Description = "The operation type (add, remove, replace, etc.)" },
                    ["path"] = new OpenApiSchema { Type = "string", Description = "The target path (e.g. /status)" },
                    ["value"] = new OpenApiSchema { Type = "object", Description = "The value to apply (for add/replace)" }
                },
                Required = new HashSet<string> { "op", "path" }
            };
            schema.Description = "JSON Patch document as per RFC 6902. Example: [{ \"op\": \"replace\", \"path\": \"/status\", \"value\": \"Active\" }]";
            schema.Properties = null;
        }
    }
}
