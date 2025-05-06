

namespace E_Commerce.Domain.Domains.Authorisation
{
	public class TempCustomer : SharedBase
	{
		public Guid TempCustomerId { get; set; }
		public string Name { get; set; }
		public string NameAr { get; set; }

		public virtual List<Cart> Carts { get; set; }

	}
}
