using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace E_Commerce.API.SwaggerConfiguration
{
    public class IgnoreRecursiveSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(TreeNode<CategoryModel>))
            {
                schema.Properties.Remove("Children"); // prevent recursion
                schema.Properties.Remove("ChildCategories"); // prevent recursion
            }
        }
    }
}
