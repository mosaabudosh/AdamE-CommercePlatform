

namespace E_Commerce.Domain.Domains
{
	public class CartDetail : SharedBase
	{
		public Guid CartDetailId { get; set; }
		public decimal Quantity { get; set; }
		public decimal? Discount { get; set; }
		public decimal Tax { get; set; }
		public decimal Price { get; set; }
		public Guid ShopItemId { get; set; }
        public Guid CartId { get; set; }

        public virtual ShopItem ShopItem { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
