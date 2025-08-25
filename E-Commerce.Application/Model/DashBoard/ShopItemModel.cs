using Adam.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class ShopItemModel
    {
        public Guid? ShopItemId { get; set; }
        public decimal? Cost { get; set; }
        public decimal? ProfitRate { get; set; }
        public decimal Price { get; set; }
        public double? InStocks { get; set; }
        public double? OutOfStocks { get; set; }
        public string? Image { get; set; }
        public int? TaxId { get; set; }
        public int ShopId { get; set; }
        public int UnitGroupId { get; set; }
        public int? ColorGroupId { get; set; }
        public int? DefaultUnitId { get; set; }
        public int? DefaultColorId { get; set; }
        public int ShopItemStatusId { get; set; }
        public string? Tax { get; set; }
        public string? ItemName { get; set; }
        public string? Shop { get; set; }
        public string? UnitGroup { get; set; }
        public string? ShopItemStatus { get; set; }
        public int? DiscountId { get; set; }
        public bool IsFeatured { get; set; }
        public bool? IsBlock { get; set; }
        public int? CategoryId { get; set; }
        public int? ShopMenuId { get; set; }

        public List<string>? Images { get; set; }
        public List<FileCreationModel>? FileCreationModels { get; set; }
    }
}
