using E_Commerce.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class ShopItemRating : SharedBase
    {
        public Guid ShopItemRatingId { get; set; }
        public decimal Rating { get; set; }
        public string Comment { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ShopItemId { get; set; }

        public virtual ShopItem ShopItem { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
