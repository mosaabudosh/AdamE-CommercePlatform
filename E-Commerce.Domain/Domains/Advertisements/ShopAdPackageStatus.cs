using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Advertisements
{
    public class ShopAdPackageStatus : SharedBase
    {
        public int ShopAdPackageStatusId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual ICollection<ShopAdPackage> ShopAdPackages { get; set; }
    }
}
