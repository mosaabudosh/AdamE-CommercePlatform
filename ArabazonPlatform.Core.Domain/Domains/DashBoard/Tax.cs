namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class Tax : SharedBase
    {
        public int TaxId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public decimal Value { get; set; }
        public string Code { get; set; }

        public virtual List<ShopItem> ShopItems { get; set; }

    }
}
