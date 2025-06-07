namespace E_Commerce.Domain.Domains.DashBoard
{
    public class Item : SharedBase
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
        public string? Image { get; set; }
        public string? Descreption { get; set; }
        public bool IsBlock { get; set; }
        public int CategoryId { get; set; }
        public int? UnitGroupId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ShopItem ShopItem { get; set; }
        public virtual UnitGroup UnitGroup { get; set; }

    }
}
