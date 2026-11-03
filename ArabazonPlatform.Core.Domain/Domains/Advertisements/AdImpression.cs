using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.Advertisements
{
    public class AdImpression : SharedBase
    {
        public int AdImpressionId { get; set; }
        public DateTime Date { get; set; }
        public int Impressions { get; set; }
        public int AdvertisementId { get; set; }

        public virtual Advertisement Advertisement { get; set; }
    }
}
