using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.Advertisements
{
    public class AdvertisementStatus : SharedBase
    {
        public int AdvertisementStatusId { get; set; }
        public string Name { get; set; }
        public string? NameAr { get; set; }

        public virtual ICollection<Advertisement> Advertisements { get; set; }
    }
}
