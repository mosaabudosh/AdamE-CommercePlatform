using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class ColorGroup : SharedBase
    {
        public int ColorGroupId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual List<Color> Colors { get; set; }
        public virtual List<ShopItem> ShopItems { get; set; }
    }
}
