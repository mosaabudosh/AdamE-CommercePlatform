using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class ShopCategory : SharedBase
    {
        public int ShopCategoryId { get; set; }
        public int ShopId { get; set; }
        public int CategoryId { get; set; }

        public virtual Shop Shop { get; set; }
        public virtual Category Category { get; set; }
    }
}
