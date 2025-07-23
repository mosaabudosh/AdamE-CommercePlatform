using E_Commerce.Domain.Domains.Authorisation;

namespace E_Commerce.Domain.Domains.DashBoard
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

        public virtual City City { get; set; }
        public virtual Shop ParentShop { get; set; }
        public virtual List<Shop> ChildShops { get; set; }
        public virtual List<User> Users { get; set; }
        public virtual List<ShopItem> ShopItems { get; set; }
        public virtual List<ShopRating> ShopRatings { get; set; }
    }
}
