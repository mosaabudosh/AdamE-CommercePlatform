using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace E_Commerce.API.SwaggerConfiguration
{
    public class SpecificControllerFilter : IDocumentFilter
    {
        private readonly Dictionary<string, string[]> _allowedEndpoints = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "Public", null },
            { "UserManagement", null },
        };

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            try
            {
                List<string> pathsToKeep = context.ApiDescriptions.Where(desc => 
                _allowedEndpoints.ContainsKey(desc.ActionDescriptor.RouteValues["controller"]) && 
                (_allowedEndpoints[desc.ActionDescriptor.RouteValues["controller"]] == null || _allowedEndpoints[desc.ActionDescriptor.RouteValues["controller"]].Contains(desc.ActionDescriptor.RouteValues["action"], StringComparer.OrdinalIgnoreCase))).Select(desc => "/" + desc.RelativePath.TrimEnd('/')).Distinct().ToList();

                List<string> pathsToRemove = swaggerDoc.Paths.Where(path => !pathsToKeep.Contains(path.Key)).Select(path => path.Key).ToList();

                foreach (var path in pathsToRemove)
                {
                    swaggerDoc.Paths.Remove(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while filtering the Swagger document: {ex.Message}");
            }

        }

    }
}
