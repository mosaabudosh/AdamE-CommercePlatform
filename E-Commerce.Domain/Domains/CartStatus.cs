

namespace E_Commerce.Domain.Domains
{
	public class CartStatus : SharedBase
	{
		public int CartStatusId { get; set; }
		public string? Name { get; set; }
		public string? NameAr { get; set; }
		public string? Code { get; set; }

		public virtual List<Cart> Carts { get; set; }
	}
}
