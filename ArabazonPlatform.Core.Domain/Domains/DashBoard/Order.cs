using ArabazonPlatform.Domain.Domains.Authentication;
using ArabazonPlatform.Domain.Domains.Authorisation;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class Order : SharedBase
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? DiscountCode { get; set; }
        public string? ReferenceNumber { get; set; }
        public int? CustomerCardInfoId { get; set; }
        public int? CustomerDeliveryAddressId { get; set; }


        public virtual OrderStatus OrderStatus { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual List<OrderDetail> OrderDetails { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual User CreationUser { get; set; }
        public virtual User LastUpdateUser { get; set; }
        public virtual User DeletedUser { get; set; }
        public virtual CustomerCardInfo CustomerCardInfo { get; set; }
        public virtual CustomerDeliveryAddress CustomerDeliveryAddress { get; set; }
    }
}
