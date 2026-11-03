using ArabazonPlatform.Domain.Domains.Advertisements;
using ArabazonPlatform.Domain.Domains.Authorisation;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class Shop : SharedBase
    {
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string CommercialName { get; set; }
        public bool HasBranch { get; set; }
        public bool IsBlock { get; set; }
        public string? Logo { get; set; }
        public string Adresse { get; set; }
        public int? NoOfBranch { get; set; }
        public int? NoOfVendor { get; set; }
        public string PhoneNumber { get; set; }
        public string PersonConcateName { get; set; }
        public string? Descreption { get; set; }
        public string? About { get; set; }
        public string Code { get; set; }
        public int? CityId { get; set; }
        public int? ParentShopId { get; set; }
        public decimal? Rating { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public bool IsViewInHome { get; set; }
        public int EstimatedDaysDelivery { get; set; }
        public decimal? DeliveryAmount { get; set; }
        public bool IsFreeReturns { get; set; }
        public bool IsCanExchanged { get; set; }

        public virtual City City { get; set; }
        public virtual Shop ParentShop { get; set; }
        public virtual ICollection<Shop> ChildShops { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ShopItem> ShopItems { get; set; }
        public virtual ICollection<ShopRating> ShopRatings { get; set; }
        public virtual ICollection<ShopCategory> ShopCategories { get; set; }
        public virtual ICollection<ShopMenu> ShopMenus { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual ICollection<ShopAdPackage> ShopAdPackages { get; set; }
    }
}
