using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.Store
{
    public class DashboardProductModel
    {
        public Guid ShopItemId { get; set; }
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public string? Code { get; set; }
        public string? Descreption { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public decimal? Quantity { get; set; }
        public int? Rating { get; set; }
        public bool? IsInFavorite { get; set; }
        public string? Shope { get; set; }
        public int? ShopId { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Tax { get; set; }
        public int? SelectedUnitId { get; set; }
        public int? UnitGroupId { get; set; }
        [JsonIgnore]
        public string? UnitIds { get; set; }
        [JsonIgnore]
        public string? UnitNames { get; set; }
        [JsonIgnore]
        public string? ColorIds { get; set; }
        [JsonIgnore]
        public string? Colors { get; set; }
        public int? SelectedColorId { get; set; }
        public string? Image { get; set; }
        public string? ImagesPaths { get; set; }
        public string? ShopeLogoPath { get; set; }
        public string? ShopeLogo { get; set; }
        public List<string>? Images { get; set; }
        public List<DDLDataSource>? UnitModels { get; set; }
        public List<DDLDataSource>? ColorModels { get; set; }

    }
}
