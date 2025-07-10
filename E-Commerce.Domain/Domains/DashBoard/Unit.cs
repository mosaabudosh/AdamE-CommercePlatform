using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class Unit : SharedBase
    {
        public int UnitId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Value { get; set; }
        public int UnitGroupId { get; set; }

        public virtual UnitGroup UnitGroup { get; set; }
        public virtual List<ShopItem> ShopItems { get; set; }
    }
}
