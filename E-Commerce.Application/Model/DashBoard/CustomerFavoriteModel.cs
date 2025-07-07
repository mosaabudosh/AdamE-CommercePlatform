using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class CustomerFavoriteModel
    {
        public Guid CustomerId { get; set; }
        public List<CustomerFavoriteDetailModel>? CustomerFavoriteDetailModels { get; set; }

    }

    public class CustomerFavoriteDetailModel
    {
        public Guid? CustomerFavoriteId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Price { get; set; }
        public Guid ShopItemId { get; set; }
    }
}
