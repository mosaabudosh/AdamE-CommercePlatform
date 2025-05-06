

using E_Commerce.Domain.Domains.Authorisation;

namespace E_Commerce.Domain.Domains
{
	public class Order : SharedBase
	{
		public Guid OrderId { get; set; }
		public Guid UserId { get; set; }
		public int OrderStatusId { get; set; }


		public virtual OrderStatus OrderStatus { get; set; }
		public virtual User User { get; set; }
		public virtual List<OrderDetail> OrderDetails { get; set; }

	}
}
