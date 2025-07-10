using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class OrderModel
    {
        public Guid? OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public int? PaymentMethodId { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentMethod { get; set; }
        public List<OrderDetailModel> OrderDetails { get; set; }

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
    }
}
