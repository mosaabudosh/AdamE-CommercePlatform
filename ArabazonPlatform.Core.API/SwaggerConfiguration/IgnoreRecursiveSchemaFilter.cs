using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Model.DashBoard;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ArabazonPlatform.API.SwaggerConfiguration
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
