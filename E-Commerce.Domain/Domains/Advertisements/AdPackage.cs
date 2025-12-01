using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Advertisements
{
    public class AdPackage : SharedBase
    {
        public int AdPackageId { get; set; }
        public string Name { get; set; }
        public string? NameAr { get; set; }
        public string Description { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int AdTypeId { get; set; }

        public virtual AdType AdType { get; set; }
        public virtual ICollection<ShopAdPackage> ShopAdPackages { get; set; }
        public virtual ICollection<Advertisement> Advertisements { get; set; }
    }
}
