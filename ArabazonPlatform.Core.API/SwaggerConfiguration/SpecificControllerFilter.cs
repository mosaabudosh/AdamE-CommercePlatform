using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ArabazonPlatform.API.SwaggerConfiguration
{
    public class SpecificControllerFilter : IDocumentFilter
    {
        private readonly Dictionary<string, string[]> _allowedEndpoints = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "ActionManagement", null },
            { "CartsManagement", null },
            { "CartStatusManagement", null },
            { "CategoryManagement", null },
            { "CityManagement", null },
            { "ColorGroupManagement", null },
            { "ColorManagement", null },
            { "CountryManagement", null },
            { "CustomerFavoriteManagement", null },
            { "CustomerManagement", null },
            { "DiscountManagement", null },
            { "Gender", null },
            { "ItemManagement", null },
            { "Language", null },
            { "NotificationManagement", null },
            { "NotificationStatusManagement", null },
            { "NotificationTypeManagement", null },
            { "OrderManagement", null },
            { "OrderStatusManagement", null },
            { "PaymentMethodManagement", null },
            { "Public", null },
            { "RoleManagement", null },
            { "RoleScreenManagement", null },
            { "ScreenAction", null },
            { "ScreenManagement", null },
            { "SettingsManagement", null },
            { "ShopItemManagement", null },
            { "ShopItemRatingManagement", null },
            { "ShopItemStatusManagement", null },
            { "ShopManagement", null },
            { "TaxManagement", null },
            { "UnitGroupManagement", null },
            { "UnitManagement", null },
            { "UserManagement", null },
            { "UserRole", null },
            { "UserTypeManagement", null },
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
