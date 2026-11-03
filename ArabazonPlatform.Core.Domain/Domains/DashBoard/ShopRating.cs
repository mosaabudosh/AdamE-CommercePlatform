using ArabazonPlatform.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class ShopRating : SharedBase
    {
        public Guid ShopRatingId { get; set; }
        public decimal Rating { get; set; }
        public int ShopId { get; set; }
        public Guid CustomerId { get; set; }

        public virtual Shop Shop { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
