using ArabazonPlatform.Domain.Domains.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ArabazonPlatform.Domain.Domains.Advertisements
{
    public class ShopAdPackage : SharedBase
    {
        public int ShopAdPackageId { get; set; }
        public int ShopId { get; set; }
        public int AdPackageId { get; set; }
        public int ShopAdPackageStatusId { get; set; }

        public virtual Shop Shop { get; set; }
        public virtual AdPackage AdPackage { get; set; }
        public virtual ShopAdPackageStatus ShopAdPackageStatus { get; set; }
        public virtual ICollection<Advertisement> Advertisements { get; set; }

    }
}
