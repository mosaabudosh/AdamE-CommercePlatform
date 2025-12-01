using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Advertisements
{
    public class AdClick : SharedBase
    {
        public int AdClickId { get; set; }
        public DateTime Date { get; set; }
        public int Clicks { get; set; }
        public int AdvertisementId { get; set; }

        public virtual Advertisement Advertisement { get; set; }
    }
}
