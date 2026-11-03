using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class ShopItemStatus : SharedBase
    {
        public int ShopItemStatusId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string? Color { get; set; }

        public virtual List<ShopItem> ShopItems { get; set; }
    }
}
