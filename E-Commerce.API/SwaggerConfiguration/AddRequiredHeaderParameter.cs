using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace E_Commerce.API.SwaggerConfiguration
{
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters is null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "Content-Type",
                In = ParameterLocation.Header,
                Description = "Content-Type"
            });

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "TimeZone-Offset",
                In = ParameterLocation.Header,
                Description = "TimeZone-Offset",
                Required = true,
            });
        }
    }
}
