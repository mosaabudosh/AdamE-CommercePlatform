using E_Commerce.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model
{
    public class CartModel
    {
        public Guid CartId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? TempCustomerId { get; set; }
        public int CartStatusId { get; set; }
        public string? User { get; set; }
        public string? TempCustomer { get; set; }
        public string? CartStatus { get; set; }
        public List<CartDetailModel>? CartDetailModels { get; set; }

    }

    public class CartDetailModel
    {
        public Guid CartDetailId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Price { get; set; }
        public Guid ShopItemId { get; set; }
        public string? ItemName { get; set; }
    }
}
