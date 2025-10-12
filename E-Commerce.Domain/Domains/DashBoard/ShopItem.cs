using E_Commerce.Domain.Domains.Authentication;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class ShopItem : SharedBase
    {
        public Guid ShopItemId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
        public string? Descreption { get; set; }
        public decimal? Cost { get; set; }
        public decimal? ProfitRate { get; set; }
        public decimal Price { get; set; }
        public double? InStocks { get; set; }
        public double? OutOfStocks { get; set; }
        public string? Image { get; set; }
        public int? TaxId { get; set; }
        public int ShopId { get; set; }
        public int UnitGroupId { get; set; }
        public int ShopItemStatusId { get; set; }
        public int? ColorGroupId { get; set; }
        public int? DefaultUnitId { get; set; }
        public int? DefaultColorId { get; set; }
        public int? DiscountId { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBlock { get; set; }
        public int CategoryId { get; set; }
        public int? ShopMenuId { get; set; }
        public int? BrandId { get; set; }

        public virtual Tax Tax { get; set; }
        public virtual List<CartDetail> CartDetails { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
        public virtual UnitGroup UnitGroup { get; set; }
        public virtual ShopItemStatus ShopItemStatus { get; set; }
        public virtual List<CustomerFavorite> CustomerFavorites { get; set; }
        public virtual ColorGroup ColorGroup { get; set; }
        public virtual Color Color { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual Discount Discount { get; set; }
        public virtual List<ShopItemRating> ShopItemRatings { get; set; }
        public virtual Category Category { get; set; }
        public virtual ShopMenu ShopMenu { get; set; }
        public virtual Brand Brand { get; set; }
    }
}
