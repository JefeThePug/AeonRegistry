
using AeonRegistry.Enums;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AeonRegistry.Filters
{
    public class EnumStringSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(String) && context.MemberInfo?.Name == "Type")
            {
                schema.Description = $"Allowed values: {String.Join(", ", Enum.GetNames<ArtifactType>())}";
            }
        }
    }
}