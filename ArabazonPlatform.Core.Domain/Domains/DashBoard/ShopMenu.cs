using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class ShopMenu : SharedBase
    {
        public int ShopMenuId { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }

        public virtual Shop Shop { get; set; }
        public virtual List<ShopItem> ShopItems { get; set; }

    }
}
