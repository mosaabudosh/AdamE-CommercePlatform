using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.Domains.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.Authentication
{
    public class CustomerFavorite : SharedBase
    {
        public Guid CustomerFavoriteId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Price { get; set; }
        public Guid ShopItemId { get; set; }
        public Guid CustomerId { get; set; }

        public virtual ShopItem ShopItem { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
