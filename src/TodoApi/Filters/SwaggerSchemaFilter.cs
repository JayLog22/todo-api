using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TodoApi.Filters;

public class SwaggerSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties == null || schema.Properties.Count == 0)
            return;

        var nullableProperties = context.Type.GetProperties()
            .Where(p => IsNullableType(p.PropertyType))
            .Select(p => p.Name)
            .ToList();

        foreach (var property in schema.Properties)
        {
            var propertyName = property.Key;
            var propertySchema = property.Value;
            
            if (nullableProperties.Any(p => string.Equals(p, propertyName, StringComparison.OrdinalIgnoreCase)))
            {
                propertySchema.Example = null;
                propertySchema.Default = null;
                
                if (propertySchema.Type == "string")
                {
                    propertySchema.Example = new OpenApiString(string.Empty);
                }
            }
        }
    }
    
    private bool IsNullableType(Type type)
    {
        if (!type.IsValueType)
            return true;
        
        return Nullable.GetUnderlyingType(type) != null;
    }
}