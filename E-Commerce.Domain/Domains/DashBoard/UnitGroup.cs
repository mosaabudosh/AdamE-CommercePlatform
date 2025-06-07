using E_Commerce.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class UnitGroup : SharedBase
    {
        public int UnitGroupId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual List<Unit> Units { get; set; }
        public virtual List<Item> Items { get; set; }
        public virtual List<ShopItem> ShopItems { get; set; }
    }
}
