using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class ShopItemRatingModel
    {
        public Guid? ShopItemRatingId { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ShopItemId { get; set; }
        public int ShopId { get; set; }
        public DateTime? Date { get; set; }
        public string? Customer { get; set; }

    }
}
