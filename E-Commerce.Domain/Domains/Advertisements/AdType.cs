using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Advertisements
{
    public class AdType : SharedBase
    {
        public int AdTypeId { get; set; }
        public string Name { get; set; }
        public string? NameAr { get; set; }

        public virtual ICollection<AdPackage> AdPackages { get; set; }
    }
}
