


namespace E_Commerce.Domain.Domains
{
	public class Tax : SharedBase
	{
		public int TaxId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }
		public decimal Percentage { get; set; }
		public decimal Value { get; set; }

		public virtual List<ShopItem> ShopItems { get; set; }

	}
}
