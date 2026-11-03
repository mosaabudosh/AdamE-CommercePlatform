using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.DashBoard
{
    public class OrderModel
    {
        public Guid? OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Customer { get; set; }
        public string? UpdateByUser { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? DiscountCode { get; set; }
        public string? ReferenceNumber { get; set; }
        public int? CustomerCardInfoId { get; set; }
        public int? CustomerDeliveryAddressId { get; set; }

        public List<OrderDetailModel>? OrderDetails { get; set; }
        [JsonIgnore]
        public List<NotificationModel>? Notifications { get; set; }

    }

    public class OrderDetailModel
    {
        public Guid? OrderDetailId { get; set; }
        public double Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Price { get; set; }
        public Guid? OrderId { get; set; }
        public Guid ShopItemId { get; set; }
        public string? ItemName { get; set; }
        public string? Shope { get; set; }
        public int? ColorId { get; set; }
    }
}
