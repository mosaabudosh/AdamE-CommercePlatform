

namespace E_Commerce.Domain.Domains
{
	public class ShopItem : SharedBase
	{
		public Guid ShopItemId { get; set; }
		public decimal? Cost { get; set; }
		public decimal? ProfitRate { get; set; }
		public decimal Price { get; set; }
		public double? InStocks { get; set; }
		public double? OutOfStocks { get; set; }
        public string? Image { get; set; }
        public int? TaxId { get; set; }
		public Guid ItemId { get; set; }
        public int ShopId { get; set; }

        public virtual Tax Tax { get; set; }
		public virtual List<OrderDetail> OrderDetails { get; set; }
		public virtual Item Item { get; set; }
		public virtual List<CartDetail> CartDetails { get; set; }
		public virtual CartDetail CartDetail { get; set; }
        public virtual Shop Shop { get; set; }
    }
}
