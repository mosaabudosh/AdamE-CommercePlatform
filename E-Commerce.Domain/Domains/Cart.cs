
using E_Commerce.Domain.Domains.Authorisation;

namespace E_Commerce.Domain.Domains
{
	public class Cart : SharedBase
	{
		public Guid CartId { get; set; }
		public Guid UserId { get; set; }
		public Guid TempCustomerId { get; set; }
		public int CartStatusId { get; set; }

		public virtual User User { get; set; }
		public virtual TempCustomer TempCustomer { get; set; }
		public virtual CartStatus CartStatus { get; set; }

	}
}
