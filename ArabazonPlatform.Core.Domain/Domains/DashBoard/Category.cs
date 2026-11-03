namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class Category : SharedBase
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
        public string? Image { get; set; }
        public string? Descreption { get; set; }
        public bool IsBlock { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? GenderId { get; set; }
        public bool IsViewInHome { get; set; }
        public bool IsViewInFooter { get; set; }
        public bool IsBeauty { get; set; }
        public bool IsKids { get; set; }

        public virtual Category ParentCategory { get; set; }
        public virtual List<Category> ChildCategories { get; set; }
        public virtual List<ShopItem> ShopItems { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual List<ShopCategory> ShopCategories { get; set; }
    }
}
