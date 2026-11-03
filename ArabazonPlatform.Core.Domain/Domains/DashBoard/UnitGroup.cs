using ArabazonPlatform.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class UnitGroup : SharedBase
    {
        public int UnitGroupId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual List<Unit> Units { get; set; }
        public virtual List<ShopItem> ShopItems { get; set; }
    }
}
