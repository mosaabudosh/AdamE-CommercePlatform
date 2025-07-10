using E_Commerce.Domain.Domains.Authorisation;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class Order : SharedBase
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? DiscountCode { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }

    }
}
